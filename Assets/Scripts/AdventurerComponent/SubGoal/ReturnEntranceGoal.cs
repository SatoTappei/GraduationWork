using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ReturnEntranceGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;

        bool _isConfirmed;

        void Awake()
        {
            _description = new BilingualString(
                "�_���W�����̓����ɖ߂�", 
                "Return to the entrance."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _description;

        public override State Check()
        {
            // ��x�ł������̍��W�ɓ��B����΁A�Ⴄ���W�Ɉړ������Ƃ��Ă�������Ԃ��B
            if (_isConfirmed)
            {
                return State.Completed;
            }

            if (Blueprint.Interaction[_adventurer.Coords.y][_adventurer.Coords.x] == '<')
            {
                _isConfirmed = true;

                return State.Completed;
            }
            else
            {
                return State.Running;
            }
        }
    }
}
