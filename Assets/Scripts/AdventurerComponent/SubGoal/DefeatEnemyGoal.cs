using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatEnemyGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;

        State _confirmed;

        void Awake()
        {
            _description = new BilingualString(
                "ˆË—Š‚³‚ê‚½“G‚ð“|‚·", 
                "Defeat requested enemies roaming in the dungeon."
            );
            _adventurer = GetComponent<Adventurer>();

            _confirmed = State.Running;
        }

        public override BilingualString Description => _description;

        public override State Check()
        {
            // Œ‹‰Ê‚ªŠm’è‚µ‚½Œã‚É•Ê‚ÌŒ‹‰Ê‚ÌðŒ‚ð–ž‚½‚µ‚½‚Æ‚µ‚Ä‚àAŒ‹‰Ê‚Í•¢‚ç‚È‚¢B
            if (_confirmed == State.Completed || _confirmed == State.Failed)
            {
                return _confirmed;
            }

            // ’Eo‚Ì“ïˆÕ“x‚ªã‚ª‚Á‚Ä‚µ‚Ü‚¤‚Ì‚ÅA2‘Ì“|‚¹‚Î\•ªB
            if (_adventurer.Status.DefeatCount >= 2)
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
