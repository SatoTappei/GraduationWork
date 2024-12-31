using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class InformationStock : MonoBehaviour
    {
        List<Information> _stock;
        Queue<Information> _pending;
        ScoreEvaluateAI _scoreEvaluateAI;

        // ���ɏ����X�V����^�C�~���O�ŕۗ������܂ߑS�č폜����B
        bool _isRequestedDelete;

        public IReadOnlyList<string> Entries => _stock.Select(info => info.Text.English).ToArray();
        public IReadOnlyList<Information> Stock => _stock;
        public IEnumerable<Information> SharedStock => Stock.Where(info => info.IsShared);

        void Awake()
        {
            _stock = new List<Information>();
            _pending = new Queue<Information>();
            _scoreEvaluateAI = new ScoreEvaluateAI();
        }

        public void AddPending(BilingualString text, string source, bool isShared = true)
        {
            if (text == null) Debug.LogWarning("�ǉ����悤�Ƃ������͂�null");
            else AddPending(new Information(text, source, isShared));
        }

        public void AddPending(Information info)
        {
            _pending.Enqueue(info);
        }

        public void RequestDelete()
        {
            _isRequestedDelete = true;
        }

        // �ێ����Ă�����̎c��^�[�����X�V�A�ۗ����̏�񂪂���ꍇ��AI���]�����A����ւ���B
        public async UniTask RefreshAsync(CancellationToken token)
        {
            // ���̍폜��v������Ă����ꍇ�B
            if (_isRequestedDelete)
            {
                _isRequestedDelete = false;
                _stock.Clear();
                _pending.Clear();
            }

            // �c��^�[�������炷�B
            for (int i = 0; i < _stock.Count; i++)
            {
                _stock[i].RemainingTurn--;
            }

            // �c��^�[����0�ɂȂ������������B
            for (int i = _stock.Count - 1; i >= 0; i--)
            {
                if (_stock[i].RemainingTurn <= 0) _stock.RemoveAt(i);
            }

            // �ۗ����̏�񂪖����ꍇ�́A�ێ����Ă�����̍X�V�����Ȃ��B
            if (_pending.Count == 0) return;

            // �񓯊������ŕ]�����邽�߁A�ۗ����̒��g���ω�����ꍇ������B
            // ���̂��߁A�]���p�̃L���[�ɒ��g���ڂ��Ă����B
            Queue<Information> pendingCopy = new Queue<Information>();
            foreach (Information p in _pending)
            {
                pendingCopy.Enqueue(p);
            }
            _pending.Clear();

            // �ۗ����̏���AI���]�����A�ێ����Ă�����ɒǉ��B
            foreach (Information newInfo in pendingCopy)
            {
                // AI�ɏ���]�����Ă��炢�A�L���^�[�������߂�B
                // �Ƃ肠�����X�R�A�����1~10�^�[���̊ԂŐݒ肷��B
                newInfo.Score = await _scoreEvaluateAI.EvaluateAsync(newInfo, token);
                newInfo.RemainingTurn = Mathf.RoundToInt(newInfo.Score * 10);

                // ���ɒm���Ă�����̏ꍇ�́A�X�R�A�Ǝc��^�[�����X�V����B
                bool isReplaced = false;
                foreach (Information info in _stock)
                {
                    if (info.Text.Japanese == newInfo.Text.Japanese)
                    {
                        info.Score = newInfo.Score;
                        info.RemainingTurn = newInfo.RemainingTurn;
                        isReplaced = true;
                        break;
                    }
                }

                if (isReplaced) continue;

                // ����ȊO�̏ꍇ�͐V�����ǉ�����B
                _stock.Add(newInfo);
            }

            // �X�R�A���Ń\�[�g�B
            Sort(_stock, 0, _stock.Count - 1);

            // �ێ��ł��鐔�𒴂����ꍇ�̓X�R�A���Ⴂ���̂������B
            const int Max = 4;
            if (_stock.Count > Max) _stock.RemoveAt(Max - 1);
        }

        static void Sort(List<Information> list, int left, int right)
        {
            if (left >= right) return;

            float pivot = list[right].Score;
            int current = left;
            for (int i = left; i <= right - 1; i++)
            {
                if (list[i].Score > pivot)
                {
                    Swap(list, current, i);
                    current++;
                }
            }

            Swap(list, current, right);

            Sort(list, left, current - 1);
            Sort(list, current + 1, right);
        }

        static void Swap(List<Information> list, int a, int b)
        {
            Information x = list[a];
            list[a] = list[b];
            list[b] = x;
        }
    }
}
