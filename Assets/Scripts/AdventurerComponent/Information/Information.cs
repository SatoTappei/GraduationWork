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

        // �`���҂Ƃ̉�b�A���[�U�[����̃R�����g�ȂǁAAI���]������K�v������ꍇ�B
        public Information(BilingualString text, string source)
        {
            _text = text;
            _source = source;
            _score = 0; // AI���]�����Č��肷��B
            _turn = 0;  // AI���]�����Č��肷��B
            _isShared = true;
            _isEvaluate = true;
        }

        // �n�`�̓����ȂǁA�V�X�e���������n���ꍇ�B
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