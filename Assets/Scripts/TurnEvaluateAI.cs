using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class TurnEvaluateAI
    {
        public TurnEvaluateAI(IReadOnlyAdventurerContext _)
        {
            // そもそもAIを使っていないのでコンストラクタの引数を使わない。
        }

        public async UniTask<int> EvaluateAsync(SharedInformation information, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            // awaitする必要ないが、警告対策で一応しておく。
            await UniTask.Yield(cancellationToken: token);

            // 情報のスコアとゲームの状態から有効ターン数を評価するプロンプトが思いつかない。
            // とりあえずスコアに応じたターン数を返すようにしておく。
            return Mathf.RoundToInt(information.Score * 10);
        }
    }
}
