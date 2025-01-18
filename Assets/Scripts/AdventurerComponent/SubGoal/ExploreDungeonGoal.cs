using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExploreDungeonGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;

        bool _isConfirmed;

        void Awake()
        {
            _description = new BilingualString(
                "�_���W��������T������B",
                "Actively explore unexplored areas, defeat enemies, and bring back items."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _description;

        public override State Check()
        {
            // �o�߃^�[��������������悤�ȏ����͏����Ȃ��Ǝv�����O�̂��߁B
            // ���ʂ��m�肵���ꍇ�͏����𖞂����Ȃ��Ȃ��Ă�����Ȃ��B
            if (_isConfirmed)
            {
                return State.Completed;
            }

            if (_adventurer.Status.ElapsedTurn >= 100)
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
