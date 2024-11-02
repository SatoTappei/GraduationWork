using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatStrongEnemy : SubGoal
    {
        public static readonly BilingualString StaticText;

        static DefeatStrongEnemy()
        {
            string j = "‹­—Í‚È“G‚ð“|‚·B";
            string e = "Defeat strong enemies roaming in the dungeon.";
            StaticText = new BilingualString(j, e);
        }

        public DefeatStrongEnemy(Adventurer owner) : base(owner) { }

        public override BilingualString Text => StaticText;

        public override bool IsCompleted()
        {
            return Owner.DefeatCount >= 1;
        }
    }
}
