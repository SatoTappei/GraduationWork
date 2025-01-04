using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ItemInventory : MonoBehaviour
    {
        // Inventory�N���X�ŊǗ����Ă���A�C�e���̊e���ڂ̏���Ԃ����߂Ɏg�p�B
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
                Debug.LogWarning("�ǉ����悤�Ƃ����A�C�e����null�������B");
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
                Debug.Log("�������Ă��Ȃ��A�C�e�����폜���悤�Ƃ����B");

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
