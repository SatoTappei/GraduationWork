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
                // ���ɓK�p����Ă���ꍇ�͌��ʎ��ԉ����B
                _remainingTurn = 10; // �K���ȃ^�[�����B
            }
            else
            {
                _isApplied = true;
                _remainingTurn = 10; // �K���ȃ^�[�����B

                _blackboard.AddStatusEffect("�����������B");
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

                _blackboard.RemoveStatusEffect("�����������B");
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
