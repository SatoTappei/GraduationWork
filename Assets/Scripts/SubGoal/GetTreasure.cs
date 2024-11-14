using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GetTreasure : SubGoal
    {
        public static readonly BilingualString StaticText;

        static GetTreasure()
        {
            string j = "‚¨•ó‚ðŽè‚É“ü‚ê‚éB";
            string e = "Find the treasure chest in the dungeon and scavenge.";
            StaticText = new BilingualString(j, e);
        }

        public GetTreasure(IReadOnlyAdventurerContext context) : base(context) { }

        public override BilingualString Text => StaticText;

        public override bool IsCompleted()
        {
            return Context.TreasureCount >= 1;
        }
    }
}