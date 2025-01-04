using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ReturnEntranceGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;

        void Awake()
        {
            _description = new BilingualString(
                "ƒ_ƒ“ƒWƒ‡ƒ“‚Ì“üŒû‚É–ß‚éB", 
                "Return to the entrance."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _description;

        public override bool IsCompleted()
        {
            return Blueprint.Interaction[_adventurer.Coords.y][_adventurer.Coords.x] == '<';
        }
    }
}
