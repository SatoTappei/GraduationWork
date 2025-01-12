using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Status
    {
        int _maxHp;
        int _currentHp;
        
        int _maxEmotion;
        int _currentEmotion;
        
        int _maxHunger;
        int _currentHunger;
        
        int _attack;
        float _attackMagnification;
        
        float _speed;
        float _rotateSpeed;
        float _speedMagnification;

        int _artifactCount;
        int _treasureCount;
        int _defeatCount;
        int _elapsedTurn;

        public Status(int level)
        {
            _maxHp = CalculationFormula.GetHp(level);
            CurrentHp = _maxHp;

            _maxEmotion = 100;
            CurrentEmotion = 50;

            _maxHunger = 100;
            CurrentHunger = 0;

            _attack = CalculationFormula.GetAttack(level);
            AttackMagnification = 1.0f;

            _speed = CalculationFormula.GetSpeed(level);
            _rotateSpeed = 4.0f;
            SpeedMagnification = 1.0f;

            TreasureCount = 0;
            DefeatCount = 0;
            ElapsedTurn = 0;
        }

        public int MaxHp => _maxHp;
        public int MaxEmotion => _maxEmotion;
        public int MaxHunger => _maxHunger;

        public float TotalSpeed => _speed * SpeedMagnification;
        public float TotalRotateSpeed => _rotateSpeed * SpeedMagnification;
        public int TotalAttack => Mathf.FloorToInt(_attack * AttackMagnification);
        public bool IsDefeated => CurrentHp <= 0;
        public bool IsAlive => !IsDefeated;
        public bool IsHungry => CurrentHunger >= _maxHunger;

        public int CurrentHp
        { 
            get => _currentHp;
            set => _currentHp = Mathf.Clamp(value, 0, _maxHp); 
        }

        public int CurrentEmotion 
        { 
            get => _currentEmotion;
            set => _currentEmotion = Mathf.Clamp(value, 0, _maxEmotion); 
        }

        public int CurrentHunger 
        { 
            get => _currentHunger;
            set => _currentHunger = Mathf.Clamp(value, 0, _maxHunger); 
        }

        public float AttackMagnification 
        { 
            get => _attackMagnification;
            set => _attackMagnification = Mathf.Max(value, 0); 
        }

        public float SpeedMagnification 
        { 
            get => _speedMagnification; 
            set => _speedMagnification = Mathf.Max(value, 0); 
        }

        public int ArtifactCount
        {
            get => _artifactCount;
            set => _artifactCount = Mathf.Max(value, 0);
        }

        public int TreasureCount
        { 
            get => _treasureCount;
            set => _treasureCount = Mathf.Max(value, 0); 
        }

        public int DefeatCount 
        { 
            get => _defeatCount; 
            set => _defeatCount = Mathf.Max(value, 0); 
        }

        public int ElapsedTurn 
        { 
            get => _elapsedTurn; 
            set => _elapsedTurn = Mathf.Max(value, 0); 
        }
    }
}
