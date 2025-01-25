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
                "���̖`���҂�|��", 
                "Defeat the adventurers."
            );
            _adventurer = GetComponent<Adventurer>();

            _confirmed = State.Running;
        }

        public override BilingualString Description => _text;

        public override State Check()
        {
            // ���ʂ��m�肵����ɕʂ̌��ʂ̏����𖞂������Ƃ��Ă��A���ʂ͕���Ȃ��B
            if (_confirmed == State.Completed || _confirmed == State.Failed)
            {
                return _confirmed;
            }

            // �����ɏo�Ă���`���҂͍ő�4�l�Ȃ̂ŁA1�̓|���Ώ\���B
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
