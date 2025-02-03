using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game.FSM
{
    public class IdleState : State
    {
        SubGoalPath _subGoal;

        void Awake()
        {
            _subGoal = GetComponent<SubGoalPath>();
        }

        protected override async UniTask<string> EnterAsync(CancellationToken token)
        {
            await UniTask.Yield();
            return "Idle";
        }

        protected override async UniTask<string> ExitAsync(CancellationToken token)
        {
            await UniTask.Yield();
            return "Idle";
        }

        protected override async UniTask<string> StayAsync(CancellationToken token)
        {
            await UniTask.Yield();
            return "Idle";
        }
    }
}
