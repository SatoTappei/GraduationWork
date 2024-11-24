using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ActionLog : MonoBehaviour
    {
        Queue<string> _log;
        Blackboard _blackboard;

        public IReadOnlyCollection<string> Log => _log;

        void Awake()
        {
            _log = new Queue<string>();
            _blackboard = GetComponent<Blackboard>();
        }

        public void Add(string text)
        {
            _log.Enqueue($"Turn{_blackboard.ElapsedTurn}: {text}");

            // AIが次の行動を選択する際の精度を見ながら調整する。
            // このログの上限を増やしたからと言ってAIの精度が上がるとは限らない。
            if (_log.Count > 10) _log.Dequeue();
        }
    }
}
