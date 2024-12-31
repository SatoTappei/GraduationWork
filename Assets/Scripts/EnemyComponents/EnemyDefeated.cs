using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Game
{
    public class EnemyDefeated : BaseAction
    {
        EnemyBlackboard _blackboard;

        void Awake()
        {
            _blackboard = GetComponent<EnemyBlackboard>();
        }

        public async UniTask<bool> DefeatedAsync(CancellationToken token)
        {
            if (_blackboard.IsAlive) return false;

            // 死亡時の演出。
            if (TryGetComponent(out EnemyDefeatedEffect effect))
            {
                await effect.PlayAsync(token);
            }

            // セルから削除。
            TryGetComponent(out Enemy enemy);
            DungeonManager.RemoveActorOnCell(enemy.Coords, enemy);

            return true;
        }
    }
}
