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

                if (itemName == "軽い鍵") itemInventory.Add(new LightKey());
                else if (itemName == "重い鍵") itemInventory.Add(new HeavyKey());
                else Debug.LogWarning($"対応するアイテムが無い。スペルミス？: {itemName}");
            }
        }
    }
}
