using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Blackboard : MonoBehaviour
    {
        public AdventurerSheet AdventurerSheet { get; set; }

        public Vector2Int Coords { get; set; }
        public Vector2Int Direction { get; set; }
        
        // 体力。0になると力尽きる。
        public int MaxHp {get; set; }
        public int CurrentHp { get; set; }
        // 心情。台詞のテンションに影響する。
        public int MaxEmotion { get; set; }
        public int CurrentEmotion { get; set; }
        // 疲労。ターン経過で減少、0になると徐々に体力が減る。
        public int MaxFatigue { get; set; }
        public int CurrentFatigue { get; set; }
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
        public bool IsFatigueMax => CurrentFatigue >= MaxFatigue;

        public Sprite Icon => AdventurerSheet.Icon;
        public string FullName => AdventurerSheet.FullName;
        public string DisplayName => AdventurerSheet.DisplayName;
        public string Job => AdventurerSheet.Job;
        public string Background => AdventurerSheet.Background;
    }
}
