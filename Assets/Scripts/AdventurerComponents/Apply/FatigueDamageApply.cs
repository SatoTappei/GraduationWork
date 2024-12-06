using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FatigueDamageApply : MonoBehaviour, IStatusEffectDisplayable
    {
        Blackboard _blackboard;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
        }

        public void Damage()
        {
            // ���X�Ƀ_���[�W���󂯂Ă����B�_���[�W�ʂ͓K���B
            _blackboard.CurrentHp -= 5;
        }

        bool IStatusEffectDisplayable.IsEnabled()
        {
            return _blackboard.CurrentFatigue >= 100;
        }

        string IStatusEffectDisplayable.GetEntry()
        {
            return "��J�ő̗͂����葱����B";
        }
    }
}
