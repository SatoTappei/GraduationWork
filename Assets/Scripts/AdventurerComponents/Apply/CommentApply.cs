using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class CommentApply : MonoBehaviour
    {
        // スプレッドシートからコメントを読み込む処理は非同期で実行されている。
        // そのため、読み込み完了より先にこのメソッドを呼び出すと、コメントが流れないので注意。
        public void Reaction()
        {
            ReactionAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask ReactionAsync(CancellationToken token)
        {
            if (!CommentDisplayer.TryFind(out CommentDisplayer commentDisplayer)) return;

            // 自身へのコメントを画面に流す。
            TryGetComponent(out Blackboard blackboard);
            IReadOnlyCollection<CommentSpreadSheetData> comment = commentDisplayer.Display(blackboard.FullName);

            // 自身へのコメントが無い場合。
            if (comment == null || comment.Count == 0) return;

            //if (!TryGetComponent(out CommentReactionAI ai)) return;

            // 自身へのコメントに対する台詞と、そのコメントを受けての心情の変化をAIに出力させる。
            //CommentReactionAI.Response response = await ai.RequestReactionAsync(comment, token);

            float score = 1; // コメントの仕様書が来るまで仮の値。

            // AIが特に暴言や誹謗中傷に対して-1ばかり出力することを想定して、心情への影響はほどほどにしておく。
            // とりあえず20%上下するようにしておく。必要に応じて調整。
            float add = (blackboard.MaxEmotion / 100.0f) * (20.0f * score);
            blackboard.CurrentEmotion += Mathf.CeilToInt(add);

            // UIに反映。
            if (TryGetComponent(out StatusBarApply statusBar)) statusBar.Apply();
        }
    }
}