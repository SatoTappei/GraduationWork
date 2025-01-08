using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatBossGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;

        void Awake()
        {
            _description = new BilingualString(
                "���͂ȓG��|���B", 
                "Defeat strong enemies roaming in the dungeon."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _description;

        public override bool IsCompleted()
        {
            // �E�o�̓�Փx���オ���Ă��܂��̂ŁA1�̓|���Ώ\���B
            return _adventurer.Status.DefeatCount >= 1;
        }

        public override bool IsRetire()
        {
            return _adventurer.Status.ElapsedTurn > 100;
        }
    }
}
