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
                    // �o�t�ʂ�K���ɐݒ�B��ƂȂ�l�ɔ{����������B
                    adventurer.StatusBuff("Attack", 1.2f, default);
                    adventurer.StatusBuff("Speed", 2.0f, default);
                }
            }

            // �C�x���g���s�����O�ɕ\���B
            GameLog.Add("�V�X�e��", "���҂����`���҂����������B", GameLogColor.Green);
        }
    }
}
