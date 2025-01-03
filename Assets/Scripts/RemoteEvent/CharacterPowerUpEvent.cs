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
                    // �o�t�ʂ�K���ɐݒ�B��ƂȂ�l�ɔ{����������B
                    buff.Apply("Attack", 1.2f);
                    buff.Apply("Speed", 2.0f);
                }
            }

            // �C�x���g���s�����O�ɕ\���B
            GameLog.Add("�V�X�e��", "���҂����`���҂����������B", GameLogColor.Green);
        }
    }
}
