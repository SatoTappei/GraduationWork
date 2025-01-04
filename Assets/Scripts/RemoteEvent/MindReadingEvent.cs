using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game
{
    public class MindReadingEvent : MonoBehaviour
    {
        AdventurerSpawner _adventurerSpawner;

        void Awake()
        {
            AdventurerSpawner.TryFind(out _adventurerSpawner);
        }

        public void Execute()
        {
            // 冒険者が持つ情報を全てまとめる。
            List<Information> temp = new List<Information>();
            Adventurer[] spawned = _adventurerSpawner.Spawned.Where(a => a != null).ToArray();
            foreach (Adventurer adventurer in spawned)
            {
                if (adventurer.TryGetComponent(out HoldInformation informationStock))
                {
                    temp.AddRange(informationStock.Information.Where(info => info.IsShared));
                }
            }

            if (temp.Count == 0) return;

            // まとめた情報からランダムに情報を選び、それぞれの冒険者に伝える。
            foreach (Adventurer adventurer in spawned)
            {
                Information info = temp[Random.Range(0, temp.Count)];

                if (adventurer.TryGetComponent(out TalkReceiver talk))
                {
                    talk.Talk(info.Text, nameof(MindReadingEvent), adventurer.Coords);
                }
            }

            // イベント実行をログに表示。
            GameLog.Add("システム", "何者かが冒険者の思考を盗聴している。", GameLogColor.Green);
        }
    }
}
