using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExploreDungeon : SubGoal
    {
        public static readonly BilingualString StaticText;

        static ExploreDungeon()
        {
            string j = "�_���W��������T������B";
            string e = "Freedom.";
            StaticText = new BilingualString(j, e);
        }

        public ExploreDungeon(Adventurer owner) : base(owner) { }

        public override BilingualString Text => StaticText;

        public override bool IsCompleted()
        {
            return Owner.ElapsedTurn >= 30;
        }
    }
}
