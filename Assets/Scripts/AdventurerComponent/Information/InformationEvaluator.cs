using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using AI;

namespace Game
{
    public class InformationEvaluator
    {
        [System.Serializable]
        class RequestFormat
        {
            public string Text;
            public string Source;
        }

        AIClient _ai;

        public InformationEvaluator()
        {
            // キャラクター性を反映していないので、選び方は全員同じ。
            string prompt =
                $"# Instructions\n" +
                $"- You are a player in a game of dungeon exploration.\n" +
                $"- Information and the source of that information are given.\n" +
                $"- Determine the reliability of the information.\n" +
                $"# OutputFormat\n" +
                $"- Output the confidence level of the information as a number between 0 and 1.\n" +
                $"# OutputExample\n" +
                $"- 0.2\n" +
                $"- 1.0\n";

            _ai = new AIClient(prompt);
        }

        public async UniTask<float> EvaluateAsync(Information information, CancellationToken token)
        {
            if (information == null)
            {
                Debug.LogWarning("評価しようとした情報がnull");
                return 0;
            }
            else if (information.Text == null)
            {
                Debug.LogWarning("評価しようとした情報の文章がnull");
                return 0;
            }

            // SharedInformation型にはAIが判定するのに必要ない日本語の文章とスコア情報が含まれている。
            // リクエスト専用の型に必要な値をコピーし、その型でリクエストする。
            RequestFormat request = new RequestFormat();
            request.Text = information.Text.English;
            request.Source = information.Source;
            
            string result = await _ai.RequestAsync(JsonUtility.ToJson(request), token);
            token.ThrowIfCancellationRequested();

            // 出力された文章の中に数値以外の文字列が含まれている可能性があるので、弾いてから数値に変換。
            float score = result
                .Split()
                .Where(s => float.TryParse(s, out float _))
                .Select(t => float.Parse(t))
                .FirstOrDefault();
#if false
            Debug.Log($"情報に対するAIの評価: 情報源:{request.Source}, 情報:{request.Text}, スコア:{score}");
#endif
            return score;
        }
    }
}