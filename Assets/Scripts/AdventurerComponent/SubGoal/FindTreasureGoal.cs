using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FindTreasureGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;

        void Awake()
        {
            _description = new BilingualString(
                "�������ɓ����B", 
                "Find the treasure chest in the dungeon and scavenge."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _description;

        public override bool IsCompleted()
        {
            // �E�o�̓�Փx���オ���Ă��܂��̂ŁA1�l������Ώ\���B
            return _adventurer.Status.TreasureCount >= 1;
        }

        public override bool IsRetire()
        {
            return _adventurer.Status.ElapsedTurn > 100;
        }
    }
}