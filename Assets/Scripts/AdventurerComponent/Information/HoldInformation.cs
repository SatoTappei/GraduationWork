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

        // 次に情報を更新するタイミングで保留中を含め全て削除する。
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
        public async UniTask UpdateAsync(CancellationToken token)
        {
            // 情報の削除を要求されていた場合。
            if (_isRequestedDelete)
            {
                _isRequestedDelete = false;
                _information.Clear();
                _pending.Clear();
            }

            // 残りターンを減らす。
            for (int i = 0; i < _information.Count; i++)
            {
                _information[i].RemainingTurn--;
            }

            // 残りターンが0になった情報を消す。
            for (int i = _information.Count - 1; i >= 0; i--)
            {
                if (_information[i].RemainingTurn <= 0) _information.RemoveAt(i);
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
                newInfo.Score = await _evaluator.EvaluateAsync(newInfo, token);
                newInfo.RemainingTurn = Mathf.RoundToInt(newInfo.Score * 10);

                // 既に知っている情報の場合は、スコアと残りターンを更新する。
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

                // それ以外の場合は新しく追加する。
                _information.Add(newInfo);
            }

            // スコア順でソート。
            Sort(_information, 0, _information.Count - 1);

            // 保持できる数を超えた場合はスコアが低いものを消す。
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
