using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HealEvent : MonoBehaviour
    {
        AdventurerSpawner _spawner;

        void Awake()
        {
            _spawner = AdventurerSpawner.Find();
        }

        public void Execute(int amount)
        {
            foreach (Adventurer adventurer in _spawner.Spawned)
            {
                if (adventurer != null)
                {
                    Execute(adventurer.AdventurerSheet.FullName, amount);
                }
            }
        }

        public void Execute(string name, int amount)
        {
            foreach (Adventurer adventurer in _spawner.Spawned)
            {
                if (adventurer == null) continue;
                else if (adventurer.AdventurerSheet.FullName != name) continue;

                if(adventurer.TryGetComponent(out HealReceiver heal))
                {
                    heal.Heal(amount, default); // ‰ñ•œ—Ê‚Í“K“–B
                }
            }
        }
    }
}
