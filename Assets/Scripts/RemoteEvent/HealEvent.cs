using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HealEvent : MonoBehaviour
    {
        AdventurerSpawner _adventurerSpawner;

        void Awake()
        {
            AdventurerSpawner.TryFind(out _adventurerSpawner);
        }

        public void Execute(string name)
        {
            if (_adventurerSpawner == null) return;

            foreach (Adventurer adventurer in _adventurerSpawner.Spawned)
            {
                if (adventurer != null && adventurer.AdventurerSheet.FullName == name)
                {
                    adventurer.Heal(33); // ‰ñ•œ—Ê‚Í“K“–B
                }
            }
        }
    }
}
