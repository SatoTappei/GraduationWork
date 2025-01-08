using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatAdventurerGoal : SubGoal
    {
        BilingualString _text;
        Adventurer _adventurer;

        void Awake()
        {
            _text = new BilingualString(
                "他の冒険者を倒す。", 
                "Defeat the adventurers."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _text;

        public override bool IsCompleted()
        {
            // 同時に出てくる冒険者は最大4人なので、1体倒せば十分。
            return _adventurer.Status.DefeatCount >= 1;
        }

        public override bool IsRetire()
        {
            return _adventurer.Status.ElapsedTurn > 100;
        }
    }
}
