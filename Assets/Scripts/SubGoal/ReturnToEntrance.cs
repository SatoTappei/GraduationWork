using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ReturnToEntrance : SubGoal
    {
        public static readonly BilingualString StaticText;

        static ReturnToEntrance()
        {
            string j = "ƒ_ƒ“ƒWƒ‡ƒ“‚Ì“üŒû‚É–ß‚éB";
            string e = "Return to the entrance.";
            StaticText = new BilingualString(j, e);
        }

        public ReturnToEntrance(IReadOnlyAdventurerContext context) : base(context) { }

        public override BilingualString Text => StaticText;

        public override bool IsCompleted()
        {
            return Blueprint.Interaction[Context.Coords.y][Context.Coords.x] == '<';
        }

        public override IEnumerable<string> GetAdditionalActions()
        {
            yield return "Return To Entrance";
        }
    }
}
