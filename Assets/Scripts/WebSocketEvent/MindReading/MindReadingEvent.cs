using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game
{
    public class MindReadingEvent : MonoBehaviour
    {
        AdventurerSpawner _spawner;

        void Awake()
        {
            _spawner = AdventurerSpawner.Find();
        }

        public void Execute()
        {
            // 冒険者が持つ共有可能な情報をまとめる。
            List<Information> temp = new List<Information>();
            foreach (Adventurer adventurer in _spawner.Spawned)
            {
                if (adventurer != null && adventurer.TryGetComponent(out HoldInformation informationStock))
                {
                    temp.AddRange(informationStock.Information.Where(info => info.IsShared));
                }
            }

            if (temp.Count == 0) return;

            // まとめた情報からランダムに情報を選び、それぞれの冒険者に伝える。
            foreach (Adventurer adventurer in _spawner.Spawned)
            {
                Information info = temp[Random.Range(0, temp.Count)];

                if (adventurer != null && adventurer.TryGetComponent(out TalkReceiver talk))
                {
                    talk.Talk(info.Text, "閃いた", nameof(MindReadingEvent));
                }
            }
        }
    }
}
