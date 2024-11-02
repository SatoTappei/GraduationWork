using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game
{
    public class InformationEvaluateAI
    {
        class RequestFormat
        {
            public string Text;
            public string Source;
        }

        AIRequest _ai;

        public InformationEvaluateAI()
        {
            // キャラクター性を反映していないので、選び方は全員同じ。
            string prompt =
                $"# Instructions\n" +
                $"- You are a player in a game of dungeon exploration.\n" +
                $"- The combination of information and source is given in Json format.\n" +
                $"- It determines if the information is reliable or not and outputs only the confidence level with a value between 0 and 1.\n" +
                $"'''\n" +
                $"# OutputExample\n" +
                $"- 0.2\n" +
                $"- 1.0\n";
            _ai = AIRequestFactory.Create(prompt);
        }

        public async UniTask<float> EvaluateAsync(SharedInformation information)
        {
            // SharedInformation型にはAIが判定するのに必要ない日本語の文章とスコア情報が含まれている。
            // リクエスト専用の型に必要な値をコピーし、その型でリクエストする。
            RequestFormat request = new RequestFormat();
            request.Text = information.Text.English;
            request.Source = information.Source;
            
            string result = await _ai.RequestAsync(JsonUtility.ToJson(request));

            // 出力された文章の中に数値以外の文字列が含まれている可能性があるので、弾いてから数値に変換。
            float score = result
                .Split()
                .Where(s => float.TryParse(s, out float _))
                .Select(t => float.Parse(t))
                .FirstOrDefault();
#if true
            Debug.Log($"情報に対するAIの評価: 情報源:{request.Source}, 情報:{request.Text}, スコア:{score}");
#endif
            return score;
        }
    }
}