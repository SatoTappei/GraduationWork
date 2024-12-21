using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ReturnToEntrance : SubGoal
    {
        public const string JapaneseText = "ƒ_ƒ“ƒWƒ‡ƒ“‚Ì“üŒû‚É–ß‚éB";

        BilingualString _text;
        Adventurer _adventurer;

        void Awake()
        {
            _text = new BilingualString(JapaneseText, "Return to the entrance.");
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Text => _text;

        public override bool IsCompleted()
        {
            return Blueprint.Interaction[_adventurer.Coords.y][_adventurer.Coords.x] == '<';
        }

        public override IEnumerable<string> GetAdditionalActions()
        {
            yield return "Return To Entrance";
        }
    }
}
