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
                "‘¼‚Ì–`Œ¯Ò‚ğ“|‚·B", 
                "Defeat the adventurers."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _text;

        public override bool IsCompleted()
        {
            // “¯‚Éo‚Ä‚­‚é–`Œ¯Ò‚ÍÅ‘å4l‚È‚Ì‚ÅA1‘Ì“|‚¹‚Î\•ªB
            return _adventurer.Status.DefeatCount >= 1;
        }
    }
}
