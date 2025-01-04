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
                "���̖`���҂�|���B", 
                "Defeat the adventurers."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _text;

        public override bool IsCompleted()
        {
            // �����ɏo�Ă���`���҂͍ő�4�l�Ȃ̂ŁA1�̓|���Ώ\���B
            return _adventurer.Status.DefeatCount >= 1;
        }
    }
}
