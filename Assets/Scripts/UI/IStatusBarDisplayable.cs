using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IStatusBarDisplayable
    {
        public Sprite Icon { get; }
        public string DisplayName { get; }
        public int MaxHp { get; }
        public int CurrentHp { get; }
        public int MaxEmotion { get; }
        public int CurrentEmotion { get; }
    }
}
