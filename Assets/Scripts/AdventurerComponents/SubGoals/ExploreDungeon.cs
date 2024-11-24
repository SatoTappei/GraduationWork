using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExploreDungeon : SubGoal
    {
        BilingualString _text;
        Blackboard _blackboard;

        void Awake()
        {
            _text = new BilingualString("ƒ_ƒ“ƒWƒ‡ƒ““à‚ğ’Tõ‚·‚éB", "Freedom.");
            _blackboard = GetComponent<Blackboard>();
        }

        public override BilingualString Text => _text;

        public override bool IsCompleted()
        {
            return _blackboard.ElapsedTurn >= 30;
        }
    }
}
