using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameLog : MonoBehaviour
    {
        [SerializeField] Text _text;

        Queue<string> _log;
        StringBuilder _stringBuilder;

        void Awake()
        {
            _text.text = string.Empty;
        }

        public void Add(string message)
        {
            // ç≈ëÂçsêîÅB
            const int Max = 5;

            _log ??= new Queue<string>();
            _stringBuilder ??= new StringBuilder();
            _stringBuilder.Clear();

            _log.Enqueue(message);
            if (_log.Count > Max) _log.Dequeue();

            foreach (string s in _log) _stringBuilder.AppendLine(s);

            _text.text = _stringBuilder.ToString();
        }
    }
}
