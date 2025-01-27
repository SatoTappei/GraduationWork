using Game.ItemData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GiveItemEvent : MonoBehaviour
    {
        AdventurerSpawner _spawner;

        void Awake()
        {
            _spawner = AdventurerSpawner.Find();
        }

        public void Execute(string itemName)
        {
            foreach (Adventurer adventurer in _spawner.Spawned)
            {
                if (adventurer == null) continue;
                if (!adventurer.TryGetComponent(out ItemInventory itemInventory)) continue;

                if (itemName == "�y����") itemInventory.Add(new LightKey());
                else if (itemName == "�d����") itemInventory.Add(new HeavyKey());
                else Debug.LogWarning($"�Ή�����A�C�e���������B�X�y���~�X�H: {itemName}");
            }
        }
    }
}
