using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatAdventurerGoal : SubGoal
    {
        BilingualString _text;
        Adventurer _adventurer;

        State _confirmed;

        void Awake()
        {
            _text = new BilingualString(
                "‘¼‚Ì–`Œ¯ŽÒ‚ð“|‚·", 
                "Defeat the adventurers."
            );
            _adventurer = GetComponent<Adventurer>();

            _confirmed = State.Running;
        }

        public override BilingualString Description => _text;

        public override State Check()
        {
            // Œ‹‰Ê‚ªŠm’è‚µ‚½Œã‚É•Ê‚ÌŒ‹‰Ê‚ÌðŒ‚ð–ž‚½‚µ‚½‚Æ‚µ‚Ä‚àAŒ‹‰Ê‚Í•¢‚ç‚È‚¢B
            if (_confirmed == State.Completed || _confirmed == State.Failed)
            {
                return _confirmed;
            }

            // “¯Žž‚Éo‚Ä‚­‚é–`Œ¯ŽÒ‚ÍÅ‘å4l‚È‚Ì‚ÅA1‘Ì“|‚¹‚Î\•ªB
            if (_adventurer.Status.DefeatCount >= 1)
            {
                _confirmed = State.Completed;
                return _confirmed;
            }
            else if (_adventurer.Status.ElapsedTurn > 100)
            {
                _confirmed = State.Failed;
                return _confirmed;
            }
            else
            {
                return State.Running;
            }
        }
    }
}
