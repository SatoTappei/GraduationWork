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

        public SharedInformation(BilingualString text, string source, float score) : this(text, source)
        {
            _score = score;
        }

        public SharedInformation(BilingualString text, string source)
        {
            _text = text;
            _source = source;
        }

        public SharedInformation(string japaneseText, string englishText, string source)
        {
            _text = new BilingualString(japaneseText, englishText);
            _source = source;
        }

        public BilingualString Text => _text;
        public string Source => _source;
        public float Score { get => _score; set => _score = value; }
    }
}