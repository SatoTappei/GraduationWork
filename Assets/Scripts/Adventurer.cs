using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Adventurer : Character, IStatusBarDisplayStatus
    {
        [SerializeField] Sprite _icon;
        [SerializeField] string _displayName;

        public int ElapsedTurn { get; protected set; }
        public int TreasureCount { get; protected set; }
        public int ItemCount { get; protected set; }
        public int DefeatCount { get; protected set; }

        public Sprite Icon => _icon;
        public string DisplayName => _displayName;
        public int MaxHp => 100;
        public int CurrentHp { get; protected set; }
        public int MaxEmotion => 100;
        public int CurrentEmotion { get; protected set; }
    }
}