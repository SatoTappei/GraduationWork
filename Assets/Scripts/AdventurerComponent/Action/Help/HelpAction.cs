using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class HelpAction : SurroundingAction
    {
        Adventurer _adventurer;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
        }

        public async UniTask<ActionResult> PlayAsync(CancellationToken token)
        {
            _adventurer.Reboot();

            GameLog.Add(
                "システム",
                $"状況を整理した。",
                LogColor.White,
                _adventurer.AdventurerSheet.Number
            );

            // アニメーション等の演出を待つ処理。
            await UniTask.Yield(cancellationToken: token);

            return new ActionResult(
                "Help",
                "Success",
                "Help me!",
                _adventurer.Coords,
                _adventurer.Direction
            );
        }
    }
}
