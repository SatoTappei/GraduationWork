using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExploreDungeonGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;

        void Awake()
        {
            _description = new BilingualString(
                "ダンジョン内を探索する。",
                "Feel free to explore the dungeon."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _description;

        public override bool IsCompleted()
        {
            // 適当なターン数経過したら完了。
            return _adventurer.Status.ElapsedTurn >= 30;
        }

        public override bool IsRetire()
        {
            return _adventurer.Status.ElapsedTurn > 100;
        }
    }
}
