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
            _actions = new List<string>()
            {
                "Move North",
                "Move South",
                "Move East",
                "Move West",
                "Attack Surrounding",
                "Scavenge Surrounding",
                "Talk Surrounding"
            };
        }

        public void Add(IEnumerable<string> actions)
        {
            _actions.AddRange(actions);
        }
    }
}
