using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MadnessStatusEffect : MonoBehaviour
    {
        Blackboard _blackboard;
        AvailableActions _availableActions;

        bool _isApplied;
        int _remainingTurn;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _availableActions = GetComponent<AvailableActions>();
        }

        public void Apply()
        {
            if (_isApplied)
            {
                // 既に適用されている場合は効果時間延長。
                _remainingTurn = 10; // 適当なターン数。
            }
            else
            {
                _isApplied = true;
                _remainingTurn = 10; // 適当なターン数。

                _blackboard.AddStatusEffect("頭おかしい。");
                _availableActions.SetScore("AttackToAdventurer", 1.0f);
                _availableActions.SetScore("TalkWithAdventurer", -1.0f);
                _availableActions.SetScore("Scavenge", -1.0f);
            }
        }

        public void Remove()
        {
            if (_isApplied)
            {
                _isApplied = false;

                _blackboard.RemoveStatusEffect("頭おかしい。");
                _availableActions.ResetScore("AttackToAdventurer");
                _availableActions.ResetScore("TalkWithAdventurer");
                _availableActions.ResetScore("Scavenge");
            }
        }

        public void Tick()
        {
            if (_isApplied)
            {
                _remainingTurn--;
                _remainingTurn = Mathf.Max(_remainingTurn, 0);

                if (_remainingTurn == 0) Remove();
            }
        }
    }
}
