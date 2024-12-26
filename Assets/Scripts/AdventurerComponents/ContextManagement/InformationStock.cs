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

        // 次に情報を更新するタイミングで保留中を含め全て削除する。
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
            Queue<SharedInformation> pendingCopy = new Queue<SharedInformation>();
            foreach (SharedInformation p in _pending)
            {
                pendingCopy.Enqueue(p);
            }
            _pending.Clear();

            // Adventurerクラスの初期化後に追加されるコンポーネントなので、AwakeやStartで取得せず、
            // Adventurer側から最初に呼び出したタイミングで取得する。
            _talkThemeSelectAI ??= GetComponent<TalkThemeSelectAI>();
            _scoreEvaluateAI ??= GetComponent<ScoreEvaluateAI>();
            _turnEvaluateAI ??= GetComponent<TurnEvaluateAI>();

            // 保留中の情報をAIが評価し、保持している情報に追加。
            foreach (SharedInformation newInfo in pendingCopy)
            {
                // AIの評価でスコアと残りターンを決める。
                newInfo.Score = await _scoreEvaluateAI.EvaluateAsync(newInfo, token);
                newInfo.RemainingTurn = await _turnEvaluateAI.EvaluateAsync(newInfo, token);

                Replace(newInfo);
            }

            // スコア順でソート。
            Sort(_stock, 0, _stock.Count - 1);

            // 保持できる数を超えた場合はスコアが低いものを消す。
            const int Max = 4;
            if (_stock.Count > Max) _stock.RemoveAt(Max - 1);

            // 会話する際に相手に伝える内容をAIが選ぶ。
            _talkTheme = await _talkThemeSelectAI.SelectAsync(_stock, token);
        }

        void Replace(SharedInformation newInfo)
        {
            // 既に知っている情報の場合は、スコアと残りターンを更新する。
            foreach (SharedInformation info in _stock)
            {
                if (info.Text.Japanese == newInfo.Text.Japanese)
                {
                    info.Score = newInfo.Score;
                    info.RemainingTurn = newInfo.RemainingTurn;
                    return;
                }
            }

            // それ以外の場合は新しく追加する。
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
