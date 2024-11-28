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
            _adventurerSpawner = AdventurerSpawner.Find();
            _uiManager = UiManager.Find();
        }

        public void Execute()
        {
            foreach (Adventurer adventurer in _adventurerSpawner.Spawned)
            {
                if (adventurer != null)
                {
                    // �o�t�ʂ�K���ɐݒ�B��ƂȂ�l�ɔ{����������B
                    adventurer.StatusBuff(attack: 1.2f, speed: 1.2f);
                }
            }

            // �C�x���g���s�����O�ɕ\���B
            _uiManager.AddLog("<color=#00ff00>���҂����`���҂����������B</color>");
        }
    }
}
