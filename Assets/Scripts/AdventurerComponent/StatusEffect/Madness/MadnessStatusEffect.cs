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

        public override string Description => "�����������B";
        public override bool IsValid => _isValid;

        void Awake()
        {
            _availableActions = GetComponent<AvailableActions>();
        }

        public void Set()
        {
            if (_isValid)
            {
                // ���ɓK�p����Ă���ꍇ�͌��ʎ��ԉ����B
                _remainingTurn = 10; // �K���ȃ^�[�����B
            }
            else
            {
                _isValid = true;
                _remainingTurn = 10; // �K���ȃ^�[�����B

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
