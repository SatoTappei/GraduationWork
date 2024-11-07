using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class SharedInformation
    {
        [SerializeField] BilingualString _text;
        [SerializeField] string _source;
        [SerializeField] float _score;
        [SerializeField] int _remainingTurn;

        public SharedInformation(BilingualString text, string source)
        {
            _text = text;
            _source = source;
        }

        public SharedInformation(string japanese, string english, string source)
        {
            _text = new BilingualString(japanese, english);
            _source = source;
        }

        public BilingualString Text => _text;
        public string Source => _source;

        // デフォルトで知っている情報以外は基本的にAI側が評価して値を設定する。
        public float Score { get => _score; set => _score = value; }
        public int RemainingTurn { get => _remainingTurn; set => _remainingTurn = value; }
    }
}