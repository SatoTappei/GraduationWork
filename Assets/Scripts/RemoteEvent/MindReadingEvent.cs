using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game
{
    public class MindReadingEvent : MonoBehaviour
    {
        AdventurerSpawner _adventurerSpawner;
        GameLog _gameLog;

        void Awake()
        {
            AdventurerSpawner.TryFind(out _adventurerSpawner);
            GameLog.TryFind(out _gameLog);
        }

        public void Execute()
        {
            // 冒険者が持つ情報を全てまとめる。
            List<Information> temp = new List<Information>();
            Adventurer[] spawned = _adventurerSpawner.Spawned.Where(a => a != null).ToArray();
            foreach (Adventurer adventurer in spawned)
            {
                if (adventurer.TryGetComponent(out InformationStock informationStock))
                {
                    temp.AddRange(informationStock.SharedStock);
                }
            }

            if (temp.Count == 0) return;

            // まとめた情報からランダムに情報を選び、それぞれの冒険者に伝える。
            foreach (Adventurer adventurer in spawned)
            {
                Information info = temp[Random.Range(0, temp.Count)];
                adventurer.Talk(info.Text, nameof(MindReadingEvent), adventurer.Coords);
            }

            // イベント実行をログに表示。
            _gameLog.Add("システム", "何者かが冒険者の思考を盗聴している。", GameLogColor.Green);
        }
    }
}
