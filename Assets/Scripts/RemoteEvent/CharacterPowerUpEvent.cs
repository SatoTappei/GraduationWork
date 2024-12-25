using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CharacterPowerUpEvent : MonoBehaviour
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
            foreach (Adventurer adventurer in _adventurerSpawner.Spawned)
            {
                if (adventurer != null)
                {
                    // �o�t�ʂ�K���ɐݒ�B��ƂȂ�l�ɔ{����������B
                    adventurer.StatusBuff(attack: 1.2f, speed: 2.0f);
                }
            }

            // �C�x���g���s�����O�ɕ\���B
            _uiManager.AddLog("�V�X�e��", "���҂����`���҂����������B", GameLogColor.Green);
        }
    }
}
