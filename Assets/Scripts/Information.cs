using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class Information
    {
        [SerializeField] BilingualString _text;
        [SerializeField] string _source;
        [SerializeField] float _score;
        [SerializeField] int _remainingTurn;
        [SerializeField] bool _isShared = true;

        public Information(string japanese, string english, string source, int remainingTurn, bool isShared)
            : this(new BilingualString(japanese, english), source, remainingTurn, isShared) { }

        public Information(string japanese, string english, string source, bool isShared)
            : this(new BilingualString(japanese, english), source, isShared) { }

        public Information(BilingualString text, string source, int remainingTurn, bool isShared) 
            : this(text, source, isShared)
        {
            _remainingTurn = remainingTurn;
        }

        public Information(BilingualString text, string source, bool isShared)
        {
            _text = text;
            _source = source;
            _isShared = isShared;
        }

        public BilingualString Text => _text;
        public string Source => _source;
        public bool IsShared => _isShared;

        // 内容と情報源を基に、情報を評価するという流れを想定。
        public float Score { get => _score; set => _score = value; }
        public int RemainingTurn { get => _remainingTurn; set => _remainingTurn = value; }
    }
}