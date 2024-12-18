using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game
{
    public class MindReadingEvent : MonoBehaviour
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
            // 冒険者が持つ情報を全てまとめる。
            List<SharedInformation> temp = new List<SharedInformation>();
            Adventurer[] spawned = _adventurerSpawner.Spawned.Where(a => a != null).ToArray();
            foreach (Adventurer adventurer in spawned)
            {
                if (adventurer.TryGetComponent(out InformationStock informationStock))
                {
                    temp.AddRange(informationStock.Stock);
                }
            }

            if (temp.Count == 0) return;

            // まとめた情報からランダムに情報を選び、それぞれの冒険者に伝える。
            foreach (Adventurer adventurer in spawned)
            {
                SharedInformation info = temp[Random.Range(0, temp.Count)];
                adventurer.Talk(info.Text, nameof(MindReadingEvent), adventurer.Coords);
            }

            // イベント実行をログに表示。
            _uiManager.AddLog("<color=#22ee22>何者かが冒険者の思考を盗聴している。</color>");
        }
    }
}
