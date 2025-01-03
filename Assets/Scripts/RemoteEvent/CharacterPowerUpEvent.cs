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
                if (adventurer != null)
                {
                    // バフ量を適当に設定。基準となる値に倍率をかける。
                    adventurer.StatusBuff("Attack", 1.2f, default);
                    adventurer.StatusBuff("Speed", 2.0f, default);
                }
            }

            // イベント実行をログに表示。
            GameLog.Add("システム", "何者かが冒険者を強化した。", GameLogColor.Green);
        }
    }
}
