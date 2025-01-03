using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CharacterPowerUpEvent : MonoBehaviour
    {
        AdventurerSpawner _adventurerSpawner;

        void Awake()
        {
            AdventurerSpawner.TryFind(out _adventurerSpawner);
        }

        public void Execute()
        {
            foreach (Adventurer adventurer in _adventurerSpawner.Spawned)
            {
                if (adventurer != null && adventurer.TryGetComponent(out BuffStatusEffect buff))
                {
                    // バフ量を適当に設定。基準となる値に倍率をかける。
                    buff.Apply("Attack", 1.2f);
                    buff.Apply("Speed", 2.0f);
                }
            }

            // イベント実行をログに表示。
            GameLog.Add("システム", "何者かが冒険者を強化した。", GameLogColor.Green);
        }
    }
}
