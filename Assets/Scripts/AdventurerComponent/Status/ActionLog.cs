using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ActionLog
    {
        Queue<string> _log;

        public ActionLog()
        {
            _log = new Queue<string>();
        }

        public IReadOnlyCollection<string> Log => _log;

        public void Add(string text)
        {
            _log.Enqueue(text);

            // AIが次の行動を選択する際の精度を見ながら調整する。
            // このログの上限を増やしたからと言ってAIの精度が上がるとは限らない。
            if (_log.Count > 3) _log.Dequeue();
        }

        public void Delete()
        {
            _log.Clear();
        }
    }
}
