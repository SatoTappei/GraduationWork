using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MadnessStatusEffect : StatusEffect
    {
        AvailableActions _actions;
        int _turn;
        bool _isValid;

        public override string Description => "�����������B";
        public override bool IsValid => _isValid;

        void Awake()
        {
            _actions = GetComponent<AvailableActions>();
        }

        public void Set()
        {
            if (_isValid)
            {
                // ���ɓK�p����Ă���ꍇ�͌��ʎ��ԉ����B
                _turn = 10; // �K���ȃ^�[�����B
            }
            else
            {
                _isValid = true;
                _turn = 10; // �K���ȃ^�[�����B

                _actions.SetScore("AttackToAdventurer", 1.0f);
                _actions.SetScore("TalkWithAdventurer", -1.0f);
                _actions.SetScore("Scavenge", -1.0f);
            }
        }

        public void Remove()
        {
            if (_isValid)
            {
                _isValid = false;

                _actions.ResetScore("AttackToAdventurer");
                _actions.ResetScore("TalkWithAdventurer");
                _actions.ResetScore("Scavenge");
            }
        }

        public override void Apply()
        {
            if (_isValid)
            {
                _turn--;
                _turn = Mathf.Max(_turn, 0);

                if (_turn == 0) Remove();
            }
        }
    }
}
