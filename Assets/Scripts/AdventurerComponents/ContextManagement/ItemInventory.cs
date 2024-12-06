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

        Dictionary<string, Stack<Item>> _contents;

        void Awake()
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

        public IEnumerable<Entry> GetEntries()
        {
            foreach (KeyValuePair<string, Stack<Item>> content in _contents)
            {
                yield return new Entry(content.Key, content.Value.Count);
            }
        }
    }
}
