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
            // �`���҂�������S�Ă܂Ƃ߂�B
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

            // �܂Ƃ߂���񂩂烉���_���ɏ���I�сA���ꂼ��̖`���҂ɓ`����B
            foreach (Adventurer adventurer in spawned)
            {
                SharedInformation info = temp[Random.Range(0, temp.Count)];
                adventurer.Talk(info.Text, nameof(MindReadingEvent), adventurer.Coords);
            }

            // �C�x���g���s�����O�ɕ\���B
            _uiManager.AddLog("<color=#22ee22>���҂����`���҂̎v�l�𓐒����Ă���B</color>");
        }
    }
}
