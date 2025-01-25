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
                "ダンジョンの入口に戻る", 
                "Return to the entrance."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _description;

        public override State Check()
        {
            // 一度でも入口の座標に到達すれば、違う座標に移動したとしても完了を返す。
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
