using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class HoldInformation : MonoBehaviour
    {
        List<Information> _information;
        Queue<Information> _pending;
        InformationEvaluator _evaluator;

        // ���ɏ����X�V����^�C�~���O�ŕۗ������܂ߑS�č폜����B
        bool _isRequestedDelete;

        public IReadOnlyList<Information> Information => _information;

        void Awake()
        {
            _information = new List<Information>();
            _pending = new Queue<Information>();
            _evaluator = new InformationEvaluator();
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
        public async UniTask UpdateAsync(CancellationToken token)
        {
            // ���̍폜��v������Ă����ꍇ�B
            if (_isRequestedDelete)
            {
                _isRequestedDelete = false;
                _information.Clear();
                _pending.Clear();
            }

            // �c��^�[�������炷�B
            for (int i = 0; i < _information.Count; i++)
            {
                _information[i].RemainingTurn--;
            }

            // �c��^�[����0�ɂȂ������������B
            for (int i = _information.Count - 1; i >= 0; i--)
            {
                if (_information[i].RemainingTurn <= 0) _information.RemoveAt(i);
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
                newInfo.Score = await _evaluator.EvaluateAsync(newInfo, token);
                newInfo.RemainingTurn = Mathf.RoundToInt(newInfo.Score * 10);

                // ���ɒm���Ă�����̏ꍇ�́A�X�R�A�Ǝc��^�[�����X�V����B
                bool isReplaced = false;
                foreach (Information info in _information)
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
                _information.Add(newInfo);
            }

            // �X�R�A���Ń\�[�g�B
            Sort(_information, 0, _information.Count - 1);

            // �ێ��ł��鐔�𒴂����ꍇ�̓X�R�A���Ⴂ���̂������B
            const int Max = 4;
            if (_information.Count > Max) _information.RemoveAt(Max - 1);
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
