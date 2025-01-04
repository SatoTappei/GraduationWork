using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MadnessStatusEffect : StatusEffect
    {
        AvailableActions _availableActions;
        int _remainingTurn;
        bool _isValid;

        public override string Description => "頭おかしい。";
        public override bool IsValid => _isValid;

        void Awake()
        {
            _availableActions = GetComponent<AvailableActions>();
        }

        public void Set()
        {
            if (_isValid)
            {
                // 既に適用されている場合は効果時間延長。
                _remainingTurn = 10; // 適当なターン数。
            }
            else
            {
                _isValid = true;
                _remainingTurn = 10; // 適当なターン数。

                _availableActions.SetScore("AttackToAdventurer", 1.0f);
                _availableActions.SetScore("TalkWithAdventurer", -1.0f);
                _availableActions.SetScore("Scavenge", -1.0f);
            }
        }

        public void Remove()
        {
            if (_isValid)
            {
                _isValid = false;

                _availableActions.ResetScore("AttackToAdventurer");
                _availableActions.ResetScore("TalkWithAdventurer");
                _availableActions.ResetScore("Scavenge");
            }
        }

        public override void Apply()
        {
            if (_isValid)
            {
                _remainingTurn--;
                _remainingTurn = Mathf.Max(_remainingTurn, 0);

                if (_remainingTurn == 0) Remove();
            }
        }
    }
}
