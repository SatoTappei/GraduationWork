using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CharacterPowerUpEvent : MonoBehaviour
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
            foreach (Adventurer adventurer in _adventurerSpawner.Spawned)
            {
                if (adventurer != null)
                {
                    // �o�t�ʂ�K���ɐݒ�B��ƂȂ�l�ɔ{����������B
                    adventurer.StatusBuff(attack: 1.2f, speed: 2.0f);
                }
            }

            // �C�x���g���s�����O�ɕ\���B
            _gameLog.Add("�V�X�e��", "���҂����`���҂����������B", GameLogColor.Green);
        }
    }
}
