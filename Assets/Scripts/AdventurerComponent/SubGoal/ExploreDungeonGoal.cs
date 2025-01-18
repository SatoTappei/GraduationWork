using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExploreDungeonGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;

        bool _isConfirmed;

        void Awake()
        {
            _description = new BilingualString(
                "ダンジョン内を探索する。",
                "Actively explore unexplored areas, defeat enemies, and bring back items."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _description;

        public override State Check()
        {
            // 経過ターン数が減少するような処理は書かないと思うが念のため。
            // 結果が確定した場合は条件を満たさなくなっても覆らない。
            if (_isConfirmed)
            {
                return State.Completed;
            }

            if (_adventurer.Status.ElapsedTurn >= 100)
            {
                _isConfirmed = true;

                return State.Completed;
            }
            else
            {
                return State.Running;
            }
        }
    }
}
