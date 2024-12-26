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
        List<SharedInformation> _stock;
        Queue<SharedInformation> _pending;
        TalkThemeSelectAI _talkThemeSelectAI;
        ScoreEvaluateAI _scoreEvaluateAI;
        TurnEvaluateAI _turnEvaluateAI;
        SharedInformation _talkTheme;

        // ���ɏ����X�V����^�C�~���O�ŕۗ������܂ߑS�č폜����B
        bool _isRequestedDelete;

        public BilingualString TalkTheme => _talkTheme.Text;
        public IReadOnlyList<string> Entries => _stock.Select(info => info.Text.English).ToArray();
        public IReadOnlyList<SharedInformation> Stock => _stock;

        void Awake()
        {
            _stock = new List<SharedInformation>();
            _pending = new Queue<SharedInformation>();
        }

        public void AddPending(BilingualString text, string source)
        {
            _pending.Enqueue(new SharedInformation(text, source));
        }

        public void AddPending(SharedInformation info)
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
            Queue<SharedInformation> pendingCopy = new Queue<SharedInformation>();
            foreach (SharedInformation p in _pending)
            {
                pendingCopy.Enqueue(p);
            }
            _pending.Clear();

            // Adventurer�N���X�̏�������ɒǉ������R���|�[�l���g�Ȃ̂ŁAAwake��Start�Ŏ擾�����A
            // Adventurer������ŏ��ɌĂяo�����^�C�~���O�Ŏ擾����B
            _talkThemeSelectAI ??= GetComponent<TalkThemeSelectAI>();
            _scoreEvaluateAI ??= GetComponent<ScoreEvaluateAI>();
            _turnEvaluateAI ??= GetComponent<TurnEvaluateAI>();

            // �ۗ����̏���AI���]�����A�ێ����Ă�����ɒǉ��B
            foreach (SharedInformation newInfo in pendingCopy)
            {
                // AI�̕]���ŃX�R�A�Ǝc��^�[�������߂�B
                newInfo.Score = await _scoreEvaluateAI.EvaluateAsync(newInfo, token);
                newInfo.RemainingTurn = await _turnEvaluateAI.EvaluateAsync(newInfo, token);

                Replace(newInfo);
            }

            // �X�R�A���Ń\�[�g�B
            Sort(_stock, 0, _stock.Count - 1);

            // �ێ��ł��鐔�𒴂����ꍇ�̓X�R�A���Ⴂ���̂������B
            const int Max = 4;
            if (_stock.Count > Max) _stock.RemoveAt(Max - 1);

            // ��b����ۂɑ���ɓ`������e��AI���I�ԁB
            _talkTheme = await _talkThemeSelectAI.SelectAsync(_stock, token);
        }

        void Replace(SharedInformation newInfo)
        {
            // ���ɒm���Ă�����̏ꍇ�́A�X�R�A�Ǝc��^�[�����X�V����B
            foreach (SharedInformation info in _stock)
            {
                if (info.Text.Japanese == newInfo.Text.Japanese)
                {
                    info.Score = newInfo.Score;
                    info.RemainingTurn = newInfo.RemainingTurn;
                    return;
                }
            }

            // ����ȊO�̏ꍇ�͐V�����ǉ�����B
            _stock.Add(newInfo);
        }

        static void Sort(List<SharedInformation> list, int left, int right)
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

        static void Swap(List<SharedInformation> list, int a, int b)
        {
            SharedInformation x = list[a];
            list[a] = list[b];
            list[b] = x;
        }
    }
}
