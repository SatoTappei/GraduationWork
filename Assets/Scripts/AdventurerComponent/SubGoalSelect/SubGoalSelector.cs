using AI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    public static class SubGoalSelector
    {
        public static async UniTask<IReadOnlyList<string>> SelectAsync(AdventurerSheet sheet, CancellationToken token)
        {
            string rolePrompt;
            if (sheet == null)
            {
                Debug.LogWarning("サブゴールを決めるのに必要な冒険者データが無い。");

                rolePrompt = "ゲームのキャラクターとして振る舞い、各質問に答えてください。";
            }
            else
            {
                rolePrompt =
                    $"# 指示内容\n" +
                    $"- 以下のキャラクターになりきって各質問に答えてください。\n" +
                    $"# キャラクター\n" +
                    $"- {sheet.Age}歳の{sheet.Job}\n" +
                    $"- {sheet.Personality}\n" +
                    $"- {sheet.Motivation}\n" +
                    $"- {sheet.Weaknesses}\n" +
                    $"- {sheet.Background}\n";
            }
            AIClient ai = new AIClient(rolePrompt);

            // サブゴールは合計2つ。最後は必ず「入口に戻る」なので、1つ選んでもらう。
            // 数値で出力させているが、文字列で出力させるよう改良した方が良いかもしれない。
            string prompt =
                $"# 指示内容\n" +
                $"- キャラクターを冒険者としてダンジョン探索ゲームに登場させます。\n" +
                $"- 自身のキャラクターの設定を基に、ゲームクリアまでに必要なサブゴールを選択してください。\n" +
                $"- 以下の選択肢から1つ選択してください。\n" +
                $"# 選択肢\n" +
                $"- お宝を手に入れる 0\n" +
                $"- 依頼されたアイテムを手に入れる 1\n" +
                $"- 依頼された敵を倒す 2\n" +
                $"- ダンジョンのボスを倒す 3\n" +
                $"- 他の冒険者を倒す 4\n" +
#if true
                $"- 各選択肢の末尾の番号のみを出力してください。";
#else
                // キャラクターの背景と照らし合わせて確認する用途。
                $"- 各選択肢の末尾の番号と、その選択をした理由を出力してください。";
#endif

            string response = await ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();

            // AIからのレスポンスが出力例とは異なる場合を想定し、文字列から数字のみを抽出する。
            List<int> result =
                response
                .Split()
                .Where(s => int.TryParse(s, out int _))
                .Select(s => int.Parse(s))
                .ToList();

            // 選んだ番号に対応したサブゴールが無い場合、0番の「お宝を手に入れる」に置換する。
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] < 0 || 4 < result[i])
                {
                    result[i] = 0;
                }
            }

            // 選んだサブゴールの数が0の場合、0番の「お宝を手に入れる」で埋める。
            if (result.Count == 0)
            {
                result.Add(0);
            }

            // 選んだサブゴールの数が2つ以上の場合、先頭1つのみ選択される。
            result = result.Take(1).ToList();

            // 番号に対応するサブゴール名に再度変換。
            List<string> subGoals = new List<string>();
            foreach (int n in result)
            {
                if (n == 0) subGoals.Add("お宝を手に入れる");
                else if (n == 1) subGoals.Add("依頼されたアイテムを手に入れる");
                else if (n == 2) subGoals.Add("依頼された敵を倒す");
                else if (n == 3) subGoals.Add("ダンジョンのボスを倒す");
                else if (n == 4) subGoals.Add("他の冒険者を倒す");
                else subGoals.Add("お宝を手に入れる");
            }

            // 2つめのサブゴールは全キャラクター共通で「ダンジョンの入口に戻る」になる。
            subGoals.Add("ダンジョンの入口に戻る");

            return subGoals;
        }
    }
}