using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HungryStatusEffect : StatusEffect
    {
        Adventurer _adventurer;
        bool _isValid;

        public override string Description => "空腹で体力が減り続ける。";
        public override bool IsValid => _isValid;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
        }

        public void Set()
        {
            if (_isValid)
            {
                // まだない
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
                // ダメージ量は適当。
                _adventurer.Status.CurrentHp -= 5;
            }
        }
    }
}
