using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using AI;

namespace Game
{
    public class CommentReactionAI : MonoBehaviour
    {
        public class Response
        {
            public string Line;
            public float Score;
        }

        public async UniTask<Response> RequestReactionAsync(IReadOnlyCollection<CommentSpreadSheetData> comment, CancellationToken token)
        {
            (string line, float score) = await (RequestLineAsync(comment, token), EvaluateAsync(comment, token));
            return new Response { Line = line, Score = score };
        }

        async UniTask<string> RequestLineAsync(IReadOnlyCollection<CommentSpreadSheetData> comment, CancellationToken token)
        {
            AIClient ai = CreateAI();
            string prompt =
                $"# 指示内容\n" +
                $"- 自身のキャラクターの設定を基に、次の台詞に対する返答を考えてください。\n" +
                $"- 「{GetCommentText(comment)}」\n" +
                $"- 短い一言で台詞をお願いします。\n" +
                $"'''\n" +
                $"# 出力形式\n" +
                $"- 台詞のみをそのまま出力してください。\n" +
                $"'''\n" +
                $"# 出力例\n" +
                $"- 応援ありがとう。\n" +
                $"- めげずに頑張るぞ！\n";
            string response = await ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();

            if (response[0] == '-') response = response[1..];

            return response.Trim().Trim('「', '」');
        }

        async UniTask<float> EvaluateAsync(IReadOnlyCollection<CommentSpreadSheetData> comment, CancellationToken token)
        {
            AIClient ai = CreateAI();
            string prompt =
                $"# 指示内容\n" +
                $"- 自身のキャラクターの設定を基に、台詞「{GetCommentText(comment)}」に対し、どのような印象を持つか答えてください。\n" +
                $"- ポジティブな印象かネガティブな印象、どちらですか？\n" +
                $"'''\n" +
                $"# 出力形式\n" +
                $"- ポジティブな印象の場合は、度合いが大きいほど1に近い数値を、弱い場合は0に近い数値を出力してください。\n" +
                $"- ネガティブな印象の場合は、度合いが大きいほど-1に近い数値を、弱い場合は0に近い数値を出力してください。\n" +
                $"'''\n" +
                $"# 出力例\n" +
                $"- 1\n" +
                $"- -0.2\n";
            string response = await ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();

            if (response[0] == '-') response = response[1..];

            if (float.TryParse(response, out float result)) return result;
            else
            {
                Debug.LogWarning($"{nameof(CommentReactionAI)}の出力形式が正しくない。{response}");
                return 0;
            }
        }

        string GetCommentText(IReadOnlyCollection<CommentSpreadSheetData> comment)
        {
            // ランダムで選択することで、応援コメントが多い場合はポジティブに、
            // 暴言や誹謗中傷コメントが多い場合はネガティブになりやすい。
            int r = Random.Range(0, comment.Count);
            return comment.ElementAt(r).Comment;
        }

        AIClient CreateAI()
        {
            Blackboard blackboard = GetComponent<Blackboard>();
            string age = blackboard.AdventurerSheet.Age;
            string job = blackboard.AdventurerSheet.Job;
            string background = blackboard.AdventurerSheet.Background;
            string prompt =
                $"# 指示内容\n" +
                $"- 以下のキャラクターになりきって各質問に答えてください。\n" +
                $"'''\n" +
                $"# キャラクター\n" +
                $"- {age}歳の{job}。\n" +
                $"- {background}\n";

            return new AIClient(prompt);
        }
    }
}
