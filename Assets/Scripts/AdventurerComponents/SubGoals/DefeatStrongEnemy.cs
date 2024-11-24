using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatStrongEnemy : SubGoal
    {
        BilingualString _text;
        Blackboard _blackboard;

        void Awake()
        {
            _text = new BilingualString("ã≠óÕÇ»ìGÇì|Ç∑ÅB", "Defeat strong enemies roaming in the dungeon.");
            _blackboard = GetComponent<Blackboard>();
        }

        public override BilingualString Text => _text;

        public override bool IsCompleted()
        {
            return _blackboard.DefeatCount >= 1;
        }
    }
}
