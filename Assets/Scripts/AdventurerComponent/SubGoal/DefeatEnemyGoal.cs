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
                "依頼された敵を倒す", 
                "Defeat requested enemies roaming in the dungeon."
            );
            _adventurer = GetComponent<Adventurer>();

            _confirmed = State.Running;
        }

        public override BilingualString Description => _description;

        public override State Check()
        {
            // 結果が確定した後に別の結果の条件を満たしたとしても、結果は覆らない。
            if (_confirmed == State.Completed || _confirmed == State.Failed)
            {
                return _confirmed;
            }

            // 脱出の難易度が上がってしまうので、2体倒せば十分。
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
