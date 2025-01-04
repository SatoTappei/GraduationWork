using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ItemInventory : MonoBehaviour
    {
        // Inventoryクラスで管理しているアイテムの各項目の情報を返すために使用。
        public struct Entry
        {
            public Entry(string name, int count)
            {
                Name = name;
                Count = count;
            }

            public string Name { get; }
            public int Count { get; }
        }

        Dictionary<string, Stack<Item>> _inventory;

        void Awake()
        {
            _inventory = new Dictionary<string, Stack<Item>>();
        }

        public void Add(Item item)
        {
            if (item == null)
            {
                Debug.LogWarning("追加しようとしたアイテムがnullだった。");
            }
            else
            {
                if (!_inventory.ContainsKey(item.Name.Japanese))
                {
                    _inventory.Add(item.Name.Japanese, new Stack<Item>());
                }

                _inventory[item.Name.Japanese].Push(item);
            }
        }

        public bool Remove(string name)
        {
            if (_inventory.ContainsKey(name))
            {
                if (_inventory[name].TryPop(out Item _)) return true;
                else return false;
            }
            else
            {
                Debug.Log("所持していないアイテムを削除しようとした。");

                return false;
            }
        }

        public IEnumerable<Entry> GetEntries()
        {
            foreach (KeyValuePair<string, Stack<Item>> e in _inventory)
            {
                yield return new Entry(e.Key, e.Value.Count);
            }
        }
    }
}
