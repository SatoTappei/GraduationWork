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

            // ���S���̉��o�B
            if (TryGetComponent(out DefeatedEffect effect)) effect.Play();
            
            // ���S���̑䎌�B
            if (TryGetComponent(out LineApply line)) line.ShowLine(RequestLineType.Defeated);

            // ���o�̏I����҂B
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            // �Z������폜�B
            TryGetComponent(out Adventurer adventurer);
            DungeonManager.TryFind(out DungeonManager dungeonManager);
            dungeonManager.RemoveActorOnCell(adventurer.Coords, adventurer);

            return true;
        }
    }
}
