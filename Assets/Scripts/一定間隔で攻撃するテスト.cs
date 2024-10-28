using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 一定間隔で攻撃するテスト : Character
{
    [SerializeField] Vector2Int _spawnCoords;

    DungeonManager _dungeonManager;
    Animator _animator;

    Vector2Int _currentCoords;
    Vector2Int _currentDirection;

    public override Vector2Int Coords => _currentCoords;
    public override Vector2Int Direction => _currentDirection;

    void Awake()
    {
        _dungeonManager = DungeonManager.Find();
        _animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        _currentCoords = _spawnCoords;
        _currentDirection = Vector2Int.up;

        _dungeonManager.AddActorOnCell(_currentCoords, this);
        Cell cell = _dungeonManager.GetCell(_currentCoords);
        transform.position = cell.Position;

        StartCoroutine(UpdateAsync());
    }

    IEnumerator UpdateAsync()
    {
        while (true)
        {
            yield return AttackAsync();
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator AttackAsync()
    {
        // 周囲のセルから目標を選ぶ。
        Adventurer target = null;
        for (int i = -1; i <= 1; i++)
        {
            for (int k = -1; k <= 1; k++)
            {
                // 上下左右の4方向のみ。
                if ((i == 0 && k == 0) || Mathf.Abs(i * k) > 0) continue;

                Vector2Int neighbourCoords = new Vector2Int(_currentCoords.x + k, _currentCoords.y + i);
                Cell cell = _dungeonManager.GetCell(neighbourCoords);

                if (cell.GetActors().Count == 0) continue;

                foreach (Actor actor in cell.GetActors())
                {
                    if (actor is Adventurer adventure)
                    {
                        target = adventure;
                        break;
                    }
                }
            }

            if (target != null) break;
        }

        if (target == null) yield break;

        // 目標を向く。
        Vector3 position = _dungeonManager.GetCell(_currentCoords).Position;
        Vector3 targetPosition = _dungeonManager.GetCell(target.Coords).Position;
        Transform axis = transform.Find("ForwardAxis");
        Vector3 goalDirection = (targetPosition - position).normalized;
        Quaternion startRotation = axis.rotation;
        Quaternion goalRotation = Quaternion.LookRotation(goalDirection);
        for (float t = 0; t <= 1; t += Time.deltaTime * 1.0f)
        {
            axis.rotation = Quaternion.Lerp(startRotation, goalRotation, t * 4);

            yield return null;
        }

        _animator.Play("Attack");
        target.Damage(ID, "パンチ", 34, Coords);
    }
}
