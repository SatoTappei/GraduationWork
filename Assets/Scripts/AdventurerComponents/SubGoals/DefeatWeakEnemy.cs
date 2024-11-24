using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatWeakEnemy : SubGoal
    {
        BilingualString _text;
        Blackboard _blackboard;

        void Awake()
        {
            _text = new BilingualString("©•ª‚æ‚èã‚»‚¤‚È“G‚ğ“|‚·B", "Defeat weak enemies roaming in the dungeon.");
            _blackboard = GetComponent<Blackboard>();
        }

        public override BilingualString Text => _text;

        public override bool IsCompleted()
        {
            return _blackboard.DefeatCount >= 3;
        }
    }
}
