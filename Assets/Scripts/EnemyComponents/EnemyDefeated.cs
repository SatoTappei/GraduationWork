using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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

            // ���S���̉��o�B
            if (TryGetComponent(out EnemyDefeatedEffect effect))
            {
                await effect.PlayAsync(token);
            }

            // �Z������폜�B
            TryGetComponent(out Enemy enemy);
            DungeonManager.TryFind(out DungeonManager dungeonManager);
            dungeonManager.RemoveActorOnCell(enemy.Coords, enemy);

            return true;
        }
    }
}
