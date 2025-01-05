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
        Vector2Int _currentCoords;
        Vector2Int _currentDirection;
        bool _isInitialized;

        ActionSelector _actionSelector;
        EnemyComponent.MovementAction _movement;
        EnemyComponent.AttackAction _attack;
        EnemyComponent.DefeatedAction _defeated;

        public EnemyComponent.Status Status => _status;
        public Vector2Int SpawnCoords => _spawnCoords;
        public override Vector2Int Coords => _currentCoords;
        public override Vector2Int Direction => _currentDirection;

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

        public void Initialize(Vector2Int coords)
        {
            _status = new EnemyComponent.Status();
            _spawnCoords = coords;
            _currentCoords = coords;
            // 上以外の向きの場合、回転させる処理が必要。
            _currentDirection = Vector2Int.up;

            _isInitialized = true;
        }

        // オーバーライドしたgetterに派生クラスでsetterを追加できない。
        // とりあえず値をセットするメソッドを作った。
        public void SetCoords(Vector2Int coords) => _currentCoords = coords;
        public void SetDirection(Vector2Int direction) => _currentDirection = direction;

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
                if (selectedAction == "Idle")
                {
                    await UniTask.Yield(cancellationToken: token);
                }
                else if (selectedAction == "MoveNorth")
                {
                    await _movement.MoveAsync(Vector2Int.up, token);
                }
                else if (selectedAction == "MoveSouth")
                {
                    await _movement.MoveAsync(Vector2Int.down, token);
                }
                else if (selectedAction == "MoveEast")
                {
                    await _movement.MoveAsync(Vector2Int.right, token);
                }
                else if (selectedAction == "MoveWest")
                {
                    await _movement.MoveAsync(Vector2Int.left, token);
                }
                else if (selectedAction == "Attack")
                {
                    await _attack.AttackAsync(token);
                }
                else
                {
                    Debug.LogWarning($"対応する行動が無い。スペルミス？: {selectedAction}");
                    await UniTask.Yield(cancellationToken: token);
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