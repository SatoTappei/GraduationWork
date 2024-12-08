using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class Enemy : Character
    {
        EnemyBlackboard _blackboard;
        bool _isInitialized;

        public override Vector2Int Coords => _blackboard.Coords;
        public override Vector2Int Direction => _blackboard.Direction;

        void Awake()
        {
            _blackboard = GetComponent<EnemyBlackboard>();
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        public void Initialize(Vector2Int coords)
        {
            _blackboard.CurrentHp = _blackboard.MaxHp;
            _blackboard.SpawnCoords = coords;
            _blackboard.Coords = coords;
            _blackboard.Direction = Vector2Int.up; // 上以外の向きの場合、回転させる処理が必要。

            _isInitialized = true;
        }

        public override string Damage(string id, string weapon, int value, Vector2Int coords)
        {
            if (_isInitialized && TryGetComponent(out EnemyDamageApply damage))
            {
                return damage.Damage(id, weapon, value, coords);
            }
            else return "Miss";
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // 初期化が完了するまで待つ。
            await UniTask.WaitUntil(() => _isInitialized, cancellationToken: token);

            // 生成したセル上に自身を移動と追加。
            DungeonManager.TryFind(out DungeonManager dungeonManager);
            dungeonManager.AddActorOnCell(Coords, this);
            transform.position = dungeonManager.GetCell(Coords).Position;

            // 湧いた際の演出。
            if (TryGetComponent(out EnemySpawnEffect spawnEffect)) spawnEffect.Play();

            TryGetComponent(out EnemyAI enemyAI);
            TryGetComponent(out EnemyMovement movement);
            TryGetComponent(out EnemyAttack attack);
            TryGetComponent(out EnemyDefeated defeated);
            while (!token.IsCancellationRequested)
            {
                // 次の行動を選択し、実行。
                string choice = enemyAI.ChoiceNextAction();
                if (choice == "Idle") await UniTask.Yield(cancellationToken: token);
                else if (choice == "Move North") await movement.MoveAsync(Vector2Int.up, token);
                else if (choice == "Move South") await movement.MoveAsync(Vector2Int.down, token);
                else if (choice == "Move East") await movement.MoveAsync(Vector2Int.right, token);
                else if (choice == "Move West") await movement.MoveAsync(Vector2Int.left, token);
                else if (choice == "Attack Surrounding") await attack.AttackAsync(token);

                // 撃破された場合。
                if (await defeated.DefeatedAsync(token)) break;

                await UniTask.Yield(cancellationToken: token);
            }

            Destroy(gameObject);
        }
    }
}