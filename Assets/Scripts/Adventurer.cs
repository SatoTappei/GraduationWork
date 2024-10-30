using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Adventurer : Character, IStatusBarDisplayStatus, IProfileWindowDisplayStatus, ITalkable
    {
        [SerializeField] AdventurerSheet _adventurerSheet;

        public int ElapsedTurn { get; protected set; }
        public int TreasureCount { get; protected set; }
        public int ItemCount { get; protected set; }
        public int DefeatCount { get; protected set; }
        public string Goal { get; protected set; }
        public string[] Items { get; protected set; }
        public string[] Infomation { get; protected set; }
        public int CurrentHp { get; protected set; }
        public int CurrentEmotion { get; protected set; }

        public Sprite Icon => _adventurerSheet.Icon;
        public string FullName => _adventurerSheet.FullName;
        public string DisplayName => _adventurerSheet.DisplayName;
        public string Job => _adventurerSheet.Job;
        public string Background => _adventurerSheet.Background;
        public int MaxHp => 100;
        public int MaxEmotion => 100;


        public virtual void Talk(string id, string topic, Vector2Int coords) { }
    }
}