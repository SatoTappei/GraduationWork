using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MadnessEvent : MonoBehaviour
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
                if (adventurer != null && adventurer.TryGetComponent(out IDamageable damage))
                {
                    damage.Damage(0, default, "Madness");
                }
            }
        }
    }
}
