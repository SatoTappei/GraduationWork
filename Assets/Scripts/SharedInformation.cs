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

        public SharedInformation(BilingualString text, string source, int remainingTurn)
            : this(text, source)
        {
            _remainingTurn = remainingTurn;
        }

        public SharedInformation(string japanese, string english, string source) 
            : this(new BilingualString(japanese, english), source) { }

        public SharedInformation(string japanese, string english, string source, int remainingTurn)
            : this(new BilingualString(japanese, english), source, remainingTurn) { }

        public BilingualString Text => _text;
        public string Source => _source;

        // ���e�Ə�񌹂���ɁA����]������Ƃ��������z��B
        public float Score { get => _score; set => _score = value; }
        public int RemainingTurn { get => _remainingTurn; set => _remainingTurn = value; }
    }
}