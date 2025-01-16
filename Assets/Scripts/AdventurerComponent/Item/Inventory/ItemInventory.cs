using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.ItemData;

namespace Game
{
    public class ItemInventory : MonoBehaviour
    {
        Dictionary<string, List<Item>> _items;
        Dictionary<string, IReadOnlyList<Item>> _copy;

        void Awake()
        {
            _items = new Dictionary<string, List<Item>>();
            _copy = new Dictionary<string, IReadOnlyList<Item>>();
        }

        public void Add(Item item)
        {
            if (item == null)
            {
                Debug.LogWarning("追加しようとしたアイテムがnullだった。");
            }
            else
            {
                if (!_items.ContainsKey(item.Name.Japanese))
                {
                    _items.Add(item.Name.Japanese, new List<Item>());
                }

                _items[item.Name.Japanese].Add(item);
            }
        }

        public bool Remove(string name)
        {
            if (_items.ContainsKey(name) && 0 < _items[name].Count)
            {
                // 同じ種類のアイテムのうち末尾の1つを削除している。
                // 個々のアイテムが状態を持たないことが前提。
                _items[name].RemoveAt(_items[name].Count - 1);

                // 残り0個になったアイテムはリスト自体を削除しておく。
                // 取得した際に「このアイテムを0個持っている」状態になってしまうのを防ぐ。
                if (_items[name].Count == 0)
                {
                    _items.Remove(name);
                }

                return true;
            }
            else
            {
                Debug.Log("所持していないアイテムを削除しようとした。");

                return false;
            }
        }

        public IReadOnlyDictionary<string, IReadOnlyList<Item>> Get()
        {
            _copy.Clear();
            foreach(KeyValuePair<string, List<Item>> e in _items)
            {
                if (e.Value.Count == 0)
                {
                    Debug.LogWarning("アイテムを0個持っている状態になっているけど大丈夫？");
                }
                else
                {
                    _copy.Add(e.Key, e.Value);
                }
            }
            
            return _copy;
        }

        public IEnumerable<ItemEntry> GetEntries()
        {
            foreach (KeyValuePair<string, IReadOnlyList<Item>> e in Get())
            {
                yield return new ItemEntry(e.Key, e.Value.Count);
            }
        }
    }
}
