using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class SubGoalPlannerAI
    {
        AIRequest _ai;

        public SubGoalPlannerAI(IReadOnlyAdventurerContext context)
        {
            // キャラクターとして振る舞うAIは台詞や背景などをUIに表示するので日本語。
            string age = context.AdventurerSheet.Age;
            string job = context.AdventurerSheet.Job;
            string background = context.AdventurerSheet.Background;
            string prompt =
                $"# 指示内容\n" +
                $"- 以下のキャラクターになりきって各質問に答えてください。\n" +
                $"'''\n" +
                $"# キャラクター\n" +
                $"- {age}歳の{job}。\n" +
                $"- {background}\n";

            _ai = AIRequestFactory.Create(prompt);
        }

        public async UniTask<IReadOnlyList<string>> RequestAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            string prompt =
                $"# 指示内容\n" +
                $"- キャラクターを冒険者としてダンジョン探索ゲームに登場させます。\n" +
                $"- 自身のキャラクターの設定を基に、ゲームクリアまでに必要なサブゴールを選択してください。\n" +
                $"- 以下の選択肢から合計3つ選択してください。\n" +
                $"- ダンジョンから脱出するために、3つめは「{ReturnToEntrance.StaticText.Japanese}」を選ぶことを推薦します。\n" +
                $"'''\n" +
                $"# 選択肢\n" +
                $"- {GetTreasure.StaticText.Japanese} 0\n" +
                $"- {GetRequestedItem.StaticText.Japanese} 1\n" +
                $"- {ExploreDungeon.StaticText.Japanese} 2\n" +
                $"- {DefeatWeakEnemy.StaticText.Japanese} 3\n" +
                $"- {DefeatStrongEnemy.StaticText.Japanese} 4\n" +
                $"- {DefeatAdventurer.StaticText.Japanese} 5\n" +
                $"- {ReturnToEntrance.StaticText.Japanese} 6\n" +
                $"'''\n" +
                $"# 出力形式\n" +
#if true
                $"- 各選択肢の末尾の番号のみを半角スペース区切りで出力してください。\n" +
#else
                // キャラクターの背景と照らし合わせて確認する用途。
                $"- 各選択肢の末尾の番号と、その選択をした理由を出力してください。\n" +
#endif
                $"'''\n" +
                $"# 出力例\n" +
                $"- 1 3 6\n" +
                $"- 4 5 6\n";

            string response = await _ai.RequestAsync(prompt);

            // AIからのレスポンスが出力例とは異なる場合を想定し、文字列から数字のみを抽出する。
            List<string> result = response.Split().Where(s => int.TryParse(s, out int _)).ToList();

            // AIの出力方法が正常な場合、AIが選択した3つの番号のみが配列に格納されている。
            if (result.Count != 3)
            {
                Debug.LogError($"適切な数のサブゴールが設定されていない。: {string.Join(",", result)}");
            }

            // 数字をそのまま返すとAIが何を選んだのか呼び出し元で把握し辛い。
            // その対策として、数字を対応するサブゴール名に変換して返す。
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] == "0") result[i] = GetTreasure.StaticText.Japanese;
                else if (result[i] == "1") result[i] = GetRequestedItem.StaticText.Japanese;
                else if (result[i] == "2") result[i] = ExploreDungeon.StaticText.Japanese;
                else if (result[i] == "3") result[i] = DefeatWeakEnemy.StaticText.Japanese;
                else if (result[i] == "4") result[i] = DefeatStrongEnemy.StaticText.Japanese;
                else if (result[i] == "5") result[i] = DefeatAdventurer.StaticText.Japanese;
                else if (result[i] == "6") result[i] = ReturnToEntrance.StaticText.Japanese;
            }

            return result;
        }
    }
}
