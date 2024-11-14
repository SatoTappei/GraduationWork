using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatWeakEnemy : SubGoal
    {
        public static readonly BilingualString StaticText;

        static DefeatWeakEnemy()
        {
            string j = "自分より弱そうな敵を倒す。";
            string e = "Defeat weak enemies roaming in the dungeon.";
            StaticText = new BilingualString(j, e);
        }

        public DefeatWeakEnemy(IReadOnlyAdventurerContext context) : base(context) { }

        public override BilingualString Text => StaticText;

        public override bool IsCompleted()
        {
            return Context.DefeatCount >= 3;
        }
    }
}
