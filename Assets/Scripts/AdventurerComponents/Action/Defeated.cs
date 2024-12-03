using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class Defeated : BaseAction
    {
        Blackboard _blackboard;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
        }

        public async UniTask<bool> DefeatedAsync(CancellationToken token)
        {
            const float PlayTime = 2.5f;

            if (_blackboard.IsAlive) return false;

            // 死亡時の演出。
            if (TryGetComponent(out DefeatedEffect effect)) effect.Play();
            
            // 死亡時の台詞。
            if (TryGetComponent(out LineApply line)) line.ShowLine(RequestLineType.Defeated);

            // 演出の終了を待つ。
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            // セルから削除。
            TryGetComponent(out Adventurer adventurer);
            DungeonManager.TryFind(out DungeonManager dungeonManager);
            dungeonManager.RemoveActorOnCell(adventurer.Coords, adventurer);

            return true;
        }
    }
}
