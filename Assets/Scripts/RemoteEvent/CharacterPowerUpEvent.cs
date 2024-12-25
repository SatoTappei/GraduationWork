using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CharacterPowerUpEvent : MonoBehaviour
    {
        AdventurerSpawner _adventurerSpawner;
        UiManager _uiManager;

        void Awake()
        {
            AdventurerSpawner.TryFind(out _adventurerSpawner);
            UiManager.TryFind(out _uiManager);
        }

        public void Execute()
        {
            foreach (Adventurer adventurer in _adventurerSpawner.Spawned)
            {
                if (adventurer != null)
                {
                    // バフ量を適当に設定。基準となる値に倍率をかける。
                    adventurer.StatusBuff(attack: 1.2f, speed: 2.0f);
                }
            }

            // イベント実行をログに表示。
            _uiManager.AddLog("システム", "何者かが冒険者を強化した。", GameLogColor.Green);
        }
    }
}
