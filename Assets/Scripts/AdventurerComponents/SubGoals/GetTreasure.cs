using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GetTreasure : SubGoal
    {
        BilingualString _text;
        Blackboard _blackboard;

        void Awake()
        {
            _text = new BilingualString("‚¨•ó‚ğè‚É“ü‚ê‚éB", "Find the treasure chest in the dungeon and scavenge.");
            _blackboard = GetComponent<Blackboard>();
        }

        public override BilingualString Text => _text;

        public override bool IsCompleted()
        {
            return _blackboard.TreasureCount >= 1;
        }
    }
}