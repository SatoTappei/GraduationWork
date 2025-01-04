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

            // サブゴールは合計3つ。最後は必ず「入口に戻る」なので、2つ選んでもらう。
            // 数値で出力させているが、文字列で出力させるよう改良した方が良いかもしれない。
            string prompt =
                $"# 指示内容\n" +
                $"- キャラクターを冒険者としてダンジョン探索ゲームに登場させます。\n" +
                $"- 自身のキャラクターの設定を基に、ゲームクリアまでに必要なサブゴールを選択してください。\n" +
                $"- 以下の選択肢から2つ選択してください。\n" +
                $"# 選択肢\n" +
                $"- お宝を手に入れる 0\n" +
                $"- 依頼されたアイテムを手に入れる 1\n" +
                $"- ダンジョン内を探索する 2\n" +
                $"- 自分より弱そうな敵を倒す 3\n" +
                $"- 強力な敵を倒す 4\n" +
                $"- 他の冒険者を倒す 5\n" +
                $"# 出力形式\n" +
#if true
                $"- 各選択肢の末尾の番号のみを半角スペース区切りで出力してください。\n" +
#else
                // キャラクターの背景と照らし合わせて確認する用途。
                $"- 各選択肢の末尾の番号と、その選択をした理由を出力してください。\n" +
#endif
                $"# 出力例\n" +
                $"- 1 3\n" +
                $"- 4 5\n";

            string response = await ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();

            // AIからのレスポンスが出力例とは異なる場合を想定し、文字列から数字のみを抽出する。
            List<int> result =
                response
                .Split()
                .Where(s => int.TryParse(s, out int _))
                .Select(s => int.Parse(s))
                .ToList();

            // 選んだ番号に対応したサブゴールが無い場合、2番の「ダンジョン内を探索する」に置換する。
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] < 0 || 5 < result[i])
                {
                    result[i] = 2;
                }
            }

            // 選んだサブゴールの数が2未満の場合、2番の「ダンジョン内を探索する」で埋める。
            if (result.Count < 2)
            {
                for (int i = 2 - result.Count; i >= 0; i--)
                {
                    result.Add(2);
                }
            }

            // 選んだサブゴールの数が3つ以上の場合、先頭2つのみ選択される。
            result = result.Take(2).ToList();

            // 番号に対応するサブゴール名に再度変換。
            List<string> subGoals = new List<string>();
            foreach (int n in result)
            {
                if (n == 0) subGoals.Add("お宝を手に入れる");
                else if (n == 1) subGoals.Add("依頼されたアイテムを手に入れる");
                else if (n == 2) subGoals.Add("ダンジョン内を探索する");
                else if (n == 3) subGoals.Add("自分より弱そうな敵を倒す");
                else if (n == 4) subGoals.Add("強力な敵を倒す");
                else if (n == 5) subGoals.Add("他の冒険者を倒す");
                else subGoals.Add("ダンジョン内を探索する");
            }

            // 3つめのサブゴールは全キャラクター共通で「ダンジョンの入口に戻る」になる。
            subGoals.Add("ダンジョンの入口に戻る");

            return subGoals;
        }
    }
}