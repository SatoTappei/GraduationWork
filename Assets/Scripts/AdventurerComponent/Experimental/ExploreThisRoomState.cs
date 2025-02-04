using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game.Experimental.FSM
{
    public class ExploreThisRoomState : State
    {
        protected override async UniTask<string> EnterAsync(CancellationToken token)
        {
            await UniTask.Yield(cancellationToken: token);
            Debug.Log("ExploreThisRoomState");
            return "Idle";
        }

        protected override async UniTask<string> ExitAsync(CancellationToken token)
        {
            await UniTask.Yield(cancellationToken: token);
            Debug.Log("ExploreThisRoomState");
            return "Idle";
        }

        protected override async UniTask<string> StayAsync(CancellationToken token)
        {
            await UniTask.Yield(cancellationToken: token);
            Debug.Log("ExploreThisRoomState");
            return "MoveEast";
        }
    }
}