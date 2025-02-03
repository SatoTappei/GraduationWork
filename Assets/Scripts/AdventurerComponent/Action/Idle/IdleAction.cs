using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class IdleAction : BaseAction
    {
        Adventurer _adventurer;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
        }

        public async UniTask<ActionResult> PlayAsync(CancellationToken token)
        {
            await UniTask.Yield(cancellationToken: token);

            return new ActionResult(
                "Idle",
                "Success",
                "Thinking about what to do next.",
                _adventurer.Coords,
                _adventurer.Direction
            );
        }
    }
}
