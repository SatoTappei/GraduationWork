using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FindTreasureGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;

        void Awake()
        {
            _description = new BilingualString(
                "お宝を手に入れる。", 
                "Find the treasure chest in the dungeon and scavenge."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _description;

        public override bool IsCompleted()
        {
            // 脱出の難易度が上がってしまうので、1つ獲得すれば十分。
            return _adventurer.Status.TreasureCount >= 1;
        }

        public override bool IsRetire()
        {
            return _adventurer.Status.ElapsedTurn > 100;
        }
    }
}