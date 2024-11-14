using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class InformationEvaluator
    {
        Queue<SharedInformation> _pending;
        Queue<SharedInformation> _evaluated;
        ScoreEvaluateAI _scoreEvaluateAI;
        TurnEvaluateAI _turnEvaluateAI;

        public InformationEvaluator(IReadOnlyAdventurerContext context)
        {
            _pending = new Queue<SharedInformation>();
            _evaluated = new Queue<SharedInformation>();
            _scoreEvaluateAI = new ScoreEvaluateAI(context);
            _turnEvaluateAI = new TurnEvaluateAI(context);
        }

        // 一定間隔で未評価の情報をAIが評価し、任意のタイミングで評価済みの情報を全て取得出来る。
        async UniTask EvaluateRepeatingAsync(CancellationToken token)
        {
            const float Duration = 1.0f;

            while (!token.IsCancellationRequested)
            {
                if (_pending.TryDequeue(out SharedInformation info))
                {
                    info.Score = await _scoreEvaluateAI.EvaluateAsync(info, token);
                    info.RemainingTurn = await _turnEvaluateAI.EvaluateAsync(info, token);
                    _evaluated.Enqueue(info);
                }

                await UniTask.WaitForSeconds(Duration, cancellationToken: token);
            }
        }

        public void PushPending(SharedInformation info)
        {
            _pending.Enqueue(info);
        }

        public IEnumerable<SharedInformation> PopAllEvaluated()
        {
            while (_evaluated.Count > 0)
            {
                yield return _pending.Dequeue();
            }
        }
    }
}
