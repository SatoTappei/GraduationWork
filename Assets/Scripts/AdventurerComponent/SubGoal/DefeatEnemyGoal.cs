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
                "�˗����ꂽ�G��|���B", 
                "Defeat requested enemies roaming in the dungeon."
            );
            _adventurer = GetComponent<Adventurer>();

            _confirmed = State.Running;
        }

        public override BilingualString Description => _description;

        public override State Check()
        {
            // ���ʂ��m�肵����ɕʂ̌��ʂ̏����𖞂������Ƃ��Ă��A���ʂ͕���Ȃ��B
            if (_confirmed == State.Completed || _confirmed == State.Failed)
            {
                return _confirmed;
            }

            // �E�o�̓�Փx���オ���Ă��܂��̂ŁA2�̓|���Ώ\���B
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
