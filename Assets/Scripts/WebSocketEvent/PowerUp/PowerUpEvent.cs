using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PowerUpEvent : MonoBehaviour
    {
        AdventurerSpawner _spawner;

        void Awake()
        {
            _spawner = AdventurerSpawner.Find();
        }

        public void Execute()
        {
            foreach (Adventurer adventurer in _spawner.Spawned)
            {
                if (adventurer == null) continue;

                Execute(adventurer.AdventurerSheet.FullName);
            }
        }

        public void Execute(string name)
        {
            foreach (Adventurer adventurer in _spawner.Spawned)
            {
                if (adventurer == null) continue;
                else if (adventurer.AdventurerSheet.FullName != name) continue;

                if (adventurer.TryGetComponent(out BuffStatusEffect buff))
                {
                    // �o�t�ʂ�K���ɐݒ�B��ƂȂ�l�ɔ{����������B
                    buff.Set("Attack", 1.2f);
                    buff.Set("Speed", 2.0f);
                }
            }
        }
    }
}
