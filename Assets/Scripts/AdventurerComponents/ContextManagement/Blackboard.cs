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
        
        // 体力。0になると力尽きる。
        public int MaxHp {get; set; }
        public int CurrentHp { get; set; }
        // 心情。台詞のテンションに影響する。
        public int MaxEmotion { get; set; }
        public int CurrentEmotion { get; set; }
        // 空腹。ターン経過で増加、最大になると徐々に体力が減る。
        public int MaxHunger { get; set; }
        public int CurrentHunger { get; set; }
        // 攻撃力。
        public int Attack { get; set; }
        public float AttackMagnification { get; set; }
        // 行動速度。
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
