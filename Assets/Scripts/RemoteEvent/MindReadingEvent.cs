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
            // �`���҂�������S�Ă܂Ƃ߂�B
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

            // �܂Ƃ߂���񂩂烉���_���ɏ���I�сA���ꂼ��̖`���҂ɓ`����B
            foreach (Adventurer adventurer in spawned)
            {
                Information info = temp[Random.Range(0, temp.Count)];
                adventurer.Talk(info.Text, nameof(MindReadingEvent), adventurer.Coords);
            }

            // �C�x���g���s�����O�ɕ\���B
            _gameLog.Add("�V�X�e��", "���҂����`���҂̎v�l�𓐒����Ă���B", GameLogColor.Green);
        }
    }
}
