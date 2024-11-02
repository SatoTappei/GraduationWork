using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatAdventurer : SubGoal
    {
        public static readonly BilingualString StaticText;

        static DefeatAdventurer()
        {
            string j = "‘¼‚Ì–`Œ¯ŽÒ‚ð“|‚·B";
            string e = "Defeat the adventurers.";
            StaticText = new BilingualString(j, e);
        }

        public DefeatAdventurer(Adventurer owner) : base(owner) { }

        public override BilingualString Text => StaticText;

        public override bool IsCompleted()
        {
            return Owner.DefeatCount >= 1;
        }
    }
}
