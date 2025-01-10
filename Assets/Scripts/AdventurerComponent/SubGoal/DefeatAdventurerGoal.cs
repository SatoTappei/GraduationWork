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
                "他の冒険者を倒す。", 
                "Defeat the adventurers."
            );
            _adventurer = GetComponent<Adventurer>();

            _confirmed = State.Running;
        }

        public override BilingualString Description => _text;

        public override State Check()
        {
            // 結果が確定した後に別の結果の条件を満たしたとしても、結果は覆らない。
            if (_confirmed == State.Completed || _confirmed == State.Failed)
            {
                return _confirmed;
            }

            // 同時に出てくる冒険者は最大4人なので、1体倒せば十分。
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
