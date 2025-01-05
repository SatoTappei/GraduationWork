using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.EnemyComponent
{
    public class Status
    {
        int _maxHp;
        int _currentHp;

        public Status()
        {
            _maxHp = 100;
            _currentHp = _maxHp;
        }

        public int MaxHp => _maxHp;
        
        public int CurrentHp
        {
            get => _currentHp;
            set => _currentHp = Mathf.Clamp(value, 0, _maxHp);
        }

        public bool IsDefeated => CurrentHp <= 0;
        public bool IsAlive => !IsDefeated;
    }
}
