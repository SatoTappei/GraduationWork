using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HungryStatusEffect : StatusEffect
    {
        Adventurer _adventurer;
        bool _isValid;

        public override string Description => "�󕠂ő̗͂����葱����B";
        public override bool IsValid => _isValid;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
        }

        public void Set()
        {
            if (_isValid)
            {
                // �܂��Ȃ�
            }
            else
            {
                _isValid = true;
            }
        }

        public void Remove()
        {
            if (_isValid)
            {
                _isValid = false;
            }
        }

        public override void Apply()
        {
            if (_isValid)
            {
                // �_���[�W�ʂ͓K���B
                _adventurer.Status.CurrentHp -= 5;
            }
        }
    }
}
