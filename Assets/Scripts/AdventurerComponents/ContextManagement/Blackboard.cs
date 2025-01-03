using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Blackboard : MonoBehaviour
    {
        List<string> _statusEffects;

        void Awake()
        {
            _statusEffects = new List<string>();
        }

        public IReadOnlyList<string> StatusEffects => _statusEffects;

        public Vector2Int Coords { get; set; }
        public Vector2Int Direction { get; set; }
        
        // �̗́B0�ɂȂ�Ɨ͐s����B
        public int MaxHp {get; set; }
        public int CurrentHp { get; set; }
        // �S��B�䎌�̃e���V�����ɉe������B
        public int MaxEmotion { get; set; }
        public int CurrentEmotion { get; set; }
        // �󕠁B�^�[���o�߂ő����A�ő�ɂȂ�Ə��X�ɑ̗͂�����B
        public int MaxHunger { get; set; }
        public int CurrentHunger { get; set; }
        // �U���́B
        public int Attack { get; set; }
        public float AttackMagnification { get; set; }
        // �s�����x�B
        public float Speed { get; set; }
        public float SpeedMagnification { get; set; }
        
        public int TreasureCount { get; set; }
        public int DefeatCount { get; set; }
        public int ElapsedTurn { get; set; }

        public bool IsDefeated => CurrentHp <= 0;
        public bool IsAlive => !IsDefeated;
        public bool IsHungry => CurrentHunger >= MaxHunger;

        public void AddStatusEffect(string effect)
        {
            _statusEffects.Add(effect);
        }

        public void RemoveStatusEffect(string effect)
        {
            _statusEffects.Remove(effect);
        }
    }
}
