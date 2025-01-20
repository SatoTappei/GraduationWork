using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Game.EnemyComponent;

namespace Game
{
    public class Enemy : Actor
    {
        [SerializeField] ParticleSystem _spawnParticle;

        EnemyComponent.Status _status;
        Vector2Int _spawnCoords;
        Vector2Int _coords;
        Vector2Int _direction;
        bool _isInitialized;

        ActionSelector _actionSelector;
        EnemyComponent.MovementAction _movement;
        EnemyComponent.AttackAction _attack;
        EnemyComponent.DefeatedAction _defeated;

        public EnemyComponent.Status Status => _status;
        public Vector2Int SpawnCoords => _spawnCoords;
        public override Vector2Int Coords => _coords;
        public override Vector2Int Direction => _direction;

        void Awake()
        {
            _actionSelector = GetComponent<ActionSelector>();
            _movement = GetComponent<EnemyComponent.MovementAction>();
            _attack = GetComponent<EnemyComponent.AttackAction>();
            _defeated = GetComponent<EnemyComponent.DefeatedAction>();
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void Update()
        {
            Debug.Log("敵ドロップテスト");
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetComponent<IDamageable>().Damage(1000, Coords);
            }
        }

        public void Initialize(Vector2Int coords)
        {
            _status = new EnemyComponent.Status();
            _spawnCoords = coords;
            _coords = coords;
            // 上以外の向きの場合、回転させる処理が必要。
            _direction = Vector2Int.up;

            _isInitialized = true;
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // 初期化が完了するまで待つ。
            await UniTask.WaitUntil(() => _isInitialized, cancellationToken: token);

            // 生成したセル上に自身を移動と追加。
            DungeonManager.AddActor(Coords, this);
            transform.position = DungeonManager.GetCell(Coords).Position;

            // 湧いた際の演出。
            _spawnParticle.Play();

            // ここまでが1ターン目開始までの処理。以降の処理は毎ターン繰り返される。
            while (!token.IsCancellationRequested)
            {
                // 次の行動を選択し、実行。
                string selectedAction = _actionSelector.Select();
                ActionResult actionResult = null; 
                if (selectedAction == "Idle")
                {
                    await UniTask.Yield(cancellationToken: token);
                }
                else if (selectedAction == "MoveNorth")
                {
                    actionResult = await _movement.MoveAsync(Vector2Int.up, token);
                }
                else if (selectedAction == "MoveSouth")
                {
                    actionResult = await _movement.MoveAsync(Vector2Int.down, token);
                }
                else if (selectedAction == "MoveEast")
                {
                    actionResult = await _movement.MoveAsync(Vector2Int.right, token);
                }
                else if (selectedAction == "MoveWest")
                {
                    actionResult = await _movement.MoveAsync(Vector2Int.left, token);
                }
                else if (selectedAction == "Attack")
                {
                    actionResult = await _attack.AttackAsync(token);
                }
                else
                {
                    Debug.LogWarning($"対応する行動が無い。スペルミス？: {selectedAction}");
                    await UniTask.Yield(cancellationToken: token);
                }

                // 行動結果を反映。
                if (actionResult != null)
                {
                    DungeonManager.RemoveActor(Coords, this);
                    _coords = actionResult.Coords;
                    DungeonManager.AddActor(Coords, this);

                    _direction = actionResult.Direction;
                }

                // 撃破された場合。
                if (await _defeated.PlayAsync(token)) break;

                await UniTask.Yield(cancellationToken: token);
            }

            // セルから削除。
            DungeonManager.RemoveActor(Coords, this);

            Destroy(gameObject);
        }
    }
}