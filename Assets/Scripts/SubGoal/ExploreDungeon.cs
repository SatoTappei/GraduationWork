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

        public ExploreDungeon(IReadOnlyAdventurerContext context) : base(context) { }

        public override BilingualString Text => StaticText;

        public override bool IsCompleted()
        {
            return Context.ElapsedTurn >= 30;
        }
    }
}
