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

        // 次に情報を更新するタイミングで保留中を含め全て削除する。
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
            if (text == null) Debug.LogWarning("追加しようとした文章がnull");
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

        // 保持している情報の残りターンを更新、保留中の情報がある場合はAIが評価し、入れ替える。
        public async UniTask RefreshAsync(CancellationToken token)
        {
            // 情報の削除を要求されていた場合。
            if (_isRequestedDelete)
            {
                _isRequestedDelete = false;
                _stock.Clear();
                _pending.Clear();
            }

            // 残りターンを減らす。
            for (int i = 0; i < _stock.Count; i++)
            {
                _stock[i].RemainingTurn--;
            }

            // 残りターンが0になった情報を消す。
            for (int i = _stock.Count - 1; i >= 0; i--)
            {
                if (_stock[i].RemainingTurn <= 0) _stock.RemoveAt(i);
            }

            // 保留中の情報が無い場合は、保持している情報の更新をしない。
            if (_pending.Count == 0) return;

            // 非同期処理で評価するため、保留中の中身が変化する場合がある。
            // そのため、評価用のキューに中身を移しておく。
            Queue<Information> pendingCopy = new Queue<Information>();
            foreach (Information p in _pending)
            {
                pendingCopy.Enqueue(p);
            }
            _pending.Clear();

            // 保留中の情報をAIが評価し、保持している情報に追加。
            foreach (Information newInfo in pendingCopy)
            {
                // AIに情報を評価してもらい、有効ターンを決める。
                // とりあえずスコアを基に1~10ターンの間で設定する。
                newInfo.Score = await _scoreEvaluateAI.EvaluateAsync(newInfo, token);
                newInfo.RemainingTurn = Mathf.RoundToInt(newInfo.Score * 10);

                // 既に知っている情報の場合は、スコアと残りターンを更新する。
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

                // それ以外の場合は新しく追加する。
                _stock.Add(newInfo);
            }

            // スコア順でソート。
            Sort(_stock, 0, _stock.Count - 1);

            // 保持できる数を超えた場合はスコアが低いものを消す。
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
