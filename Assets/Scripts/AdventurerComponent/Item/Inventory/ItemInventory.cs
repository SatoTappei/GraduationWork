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
                Debug.LogWarning("�ǉ����悤�Ƃ����A�C�e����null�������B");
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
                // ������ނ̃A�C�e���̂���������1���폜���Ă���B
                // �X�̃A�C�e������Ԃ������Ȃ����Ƃ��O��B
                _items[name].RemoveAt(_items[name].Count - 1);

                // �c��0�ɂȂ����A�C�e���̓��X�g���̂��폜���Ă����B
                // �擾�����ۂɁu���̃A�C�e����0�����Ă���v��ԂɂȂ��Ă��܂��̂�h���B
                if (_items[name].Count == 0)
                {
                    _items.Remove(name);
                }

                return true;
            }
            else
            {
                Debug.Log("�������Ă��Ȃ��A�C�e�����폜���悤�Ƃ����B");

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
                    Debug.LogWarning("�A�C�e����0�����Ă����ԂɂȂ��Ă��邯�Ǒ��v�H");
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
