using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // 削除してから追加すると選択肢の並び順が変わるが、AIが行動を選択する際に影響するのか？
    public class AvailableActions : MonoBehaviour
    {
        List<string> _actions;

        public IReadOnlyList<string> Actions => _actions;

        void Awake()
        {
            _actions = new List<string>();
            SetDefault();
        }

        public void Add(IEnumerable<string> actions)
        {
            foreach (string action in actions) Add(action);
        }

        public void Add(string action)
        {
            if (_actions.Contains(action)) return;

            _actions.Add(action);
        }

        public void Remove(string action)
        {
            _actions.Remove(action);
        }

        public void SetDefault()
        {
            _actions.Clear();
            _actions.Add("Move North");
            _actions.Add("Move South");
            _actions.Add("Move East");
            _actions.Add("Move West");
            _actions.Add("Attack Surrounding");
            _actions.Add("Scavenge Surrounding");
            _actions.Add("Talk Surrounding");
        }
    }
}
