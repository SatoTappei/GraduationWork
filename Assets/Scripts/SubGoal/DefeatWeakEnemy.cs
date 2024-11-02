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
            string j = "Ž©•ª‚æ‚èŽã‚»‚¤‚È“G‚ð“|‚·B";
            string e = "Defeat weak enemies roaming in the dungeon.";
            StaticText = new BilingualString(j, e);
        }

        public DefeatWeakEnemy(Adventurer owner) : base(owner) { }

        public override BilingualString Text => StaticText;

        public override bool IsCompleted()
        {
            return Owner.DefeatCount >= 3;
        }
    }
}
