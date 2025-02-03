using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game.FSM
{
    public abstract class State : MonoBehaviour
    {
        enum Stage { Enter, Stay, Exit }

        IReadOnlyDictionary<string, State> _states;
        State _next;
        Stage _stage;
        string _id = string.Empty;

        bool _isInitialized;

        public string ID
        {
            get
            {
                if (_id == string.Empty) _id = GetType().Name;
                return _id;
            }
        }

        public void Initialize(IReadOnlyDictionary<string, State> states)
        {
            _states = states;
            _next = this;
            _stage = Stage.Enter;

            _isInitialized = true;
        }

        public async UniTask<Result> UpdateAsync(CancellationToken token)
        {
            if (_stage == Stage.Enter) // 遷移後、最初の1回だけ実行される。
            {
                // 初期化を済ませているかチェック。
                if (!_isInitialized)
                {
                    Debug.LogWarning("初期化していない状態で実行。");
                }

                string action = await EnterAsync(token);
                _stage = Stage.Stay;

                return new Result(this, action);
            }
            else if (_stage == Stage.Exit) // 遷移する直前、1回だけ実行される。
            {
                string action = await ExitAsync(token);
                _stage = Stage.Enter;

                // 遷移先が設定されているかチェック。
                if (_next == null)
                {
                    Debug.LogWarning("遷移先が存在しない。");
                }

                return new Result(_next, action);
            }
            else
            {
                string action = await StayAsync(token);
                return new Result(this, action);
            }
        }

        protected virtual async UniTask<string> EnterAsync(CancellationToken token)
        {
            await UniTask.Yield(cancellationToken: token);
            return string.Empty;
        }

        protected virtual async UniTask<string> StayAsync(CancellationToken token)
        {
            await UniTask.Yield(cancellationToken: token);
            return string.Empty;
        }

        protected virtual async UniTask<string> ExitAsync(CancellationToken token)
        {
            await UniTask.Yield(cancellationToken: token);
            return string.Empty;
        }

        protected void Transition(string next)
        {
            if (_stage == Stage.Enter)
            {
                Debug.LogWarning($"Enterが呼ばれる前にステートを遷移することは不可能。遷移先:{next}");
            }
            else if (_stage == Stage.Exit)
            {
                Debug.LogWarning($"既に別のステートに遷移する処理が呼ばれている。遷移先:{next}");
            }

            if (_states.ContainsKey(next))
            {
                _stage = Stage.Exit;
                _next = _states[next];
            }
            else
            {
                Debug.LogWarning("遷移先が登録されていない。");
            }
        }
    }
}
