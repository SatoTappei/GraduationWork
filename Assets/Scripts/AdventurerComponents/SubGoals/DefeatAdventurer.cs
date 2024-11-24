using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatAdventurer : SubGoal
    {
        BilingualString _text;
        Blackboard _blackboard;

        void Awake()
        {
            _text = new BilingualString("ëºÇÃñ`åØé“Çì|Ç∑ÅB", "Defeat the adventurers.");
            _blackboard = GetComponent<Blackboard>();
        }

        public override BilingualString Text => _text;

        public override bool IsCompleted()
        {
            return _blackboard.DefeatCount >= 1;
        }
    }
}
