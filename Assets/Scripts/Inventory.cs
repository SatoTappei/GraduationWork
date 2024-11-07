using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IReadOnlyInventory
    {
        public IEnumerable<InventoryItem> GetAllInventoryItem();
    }

    // Inventoryクラスで管理しているアイテムの各項目の情報を返すために使用。
    public struct InventoryItem
    {
        public InventoryItem(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public string Name { get; }
        public int Count { get; }
    }

    public class Inventory : IReadOnlyInventory
    {
        Dictionary<string, Stack<Item>> _contents;

        public Inventory()
        {
            _contents = new Dictionary<string, Stack<Item>>();
        }

        public void Add(Item item)
        {
            string key = item.Name.Japanese;

            if (!_contents.ContainsKey(key))
            {
                _contents.Add(key, new Stack<Item>());
            }

            _contents[key].Push(item);
        }

        public bool Remove(string name)
        {
            if (_contents.ContainsKey(name))
            {
                if (_contents[name].TryPop(out Item _)) return true;
                else return false;
            }
            else return false;
        }

        public IEnumerable<InventoryItem> GetAllInventoryItem()
        {
            foreach (KeyValuePair<string, Stack<Item>> content in _contents)
            {
                yield return new InventoryItem(content.Key, content.Value.Count);
            }
        }
    }
}
