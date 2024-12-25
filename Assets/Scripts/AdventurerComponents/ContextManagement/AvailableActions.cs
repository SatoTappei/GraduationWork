using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
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
            _actions.AddRange(actions);
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
