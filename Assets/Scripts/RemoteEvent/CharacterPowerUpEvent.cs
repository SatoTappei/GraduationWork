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
            _adventurerSpawner = AdventurerSpawner.Find();
            _uiManager = UiManager.Find();
        }

        public void Execute()
        {
            foreach (Adventurer adventurer in _adventurerSpawner.Spawned)
            {
                if (adventurer != null)
                {
                    // バフ量を適当に設定。基準となる値に倍率をかける。
                    adventurer.StatusBuff(attack: 1.2f, speed: 1.2f);
                }
            }

            // イベント実行をログに表示。
            _uiManager.AddLog("<color=#00ff00>何者かが冒険者を強化した。</color>");
        }
    }
}
