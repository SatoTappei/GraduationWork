using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatEnemyGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;

        void Awake()
        {
            _description = new BilingualString(
                "�セ���ȓG��|���B", 
                "Defeat weak enemies roaming in the dungeon."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _description;

        public override bool IsCompleted()
        {
            // �E�o�̓�Փx���オ���Ă��܂��̂ŁA1�̓|���Ώ\���B
            return _adventurer.Status.DefeatCount >= 1;
        }
    }
}
