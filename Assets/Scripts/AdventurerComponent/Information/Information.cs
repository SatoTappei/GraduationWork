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
        [SerializeField] int _turn;
        [SerializeField] bool _isShared = true;
        [SerializeField] bool _isEvaluate = true;

        // 冒険者との会話、ユーザーからのコメントなど、AIが評価する必要がある場合。
        public Information(BilingualString text, string source)
        {
            _text = text;
            _source = source;
            _score = 0; // AIが評価して決定する。
            _turn = 0;  // AIが評価して決定する。
            _isShared = true;
            _isEvaluate = true;
        }

        // 地形の特徴など、システムから情報を渡す場合。
        public Information(string japanese, string english, string source, float score, int turn)
        {
            _text = new BilingualString(japanese, english);
            _source = source;
            _score = score;
            _turn = turn;
            _isShared = false;
            _isEvaluate = false;
        }

        public BilingualString Text => _text;
        public string Source => _source;
        public bool IsShared => _isShared;
        public bool IsEvaluate => _isEvaluate;

        public float Score 
        { 
            get => _score;
            set => _score = Mathf.Max(0, value);
        }

        public int Turn 
        { 
            get => _turn; 
            set => _turn = Mathf.Max(0, value);
        }
    }
}