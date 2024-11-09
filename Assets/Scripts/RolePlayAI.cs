using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    public enum RequestLineType
    {
        Entry,              // 登場。
        Defeated,           // 撃破された。
        Goal,               // 脱出した。
        GetTreasureSuccess, // 宝物を入手。
        GetTreasureFailure, // 宝箱が空っぽ。
        GetItemSuccess,     // アイテムを入手。
        GetItemFailure,     // アイテムが無かった。
        DefeatEnemy,        // 敵を撃破した。
        Attack,             // 攻撃時の掛け声。
        Damage,             // ダメージを受けたときの呻き声。
        Greeting            // 他の冒険者に声をかける時の挨拶。
    }

    public class RolePlayAI
    {
        AIRequest _ai;

        public RolePlayAI(IRolePlayAIResource resource)
        {
            // キャラクターとして振る舞うAIは台詞や背景などをUIに表示するので日本語。
            string age = resource.AdventurerSheet.Age;
            string job = resource.AdventurerSheet.Job;
            string background = resource.AdventurerSheet.Background;
            string prompt =
                $"# 指示内容\n" +
                $"- 以下のキャラクターになりきって各質問に答えてください。\n" +
                $"'''\n" +
                $"# キャラクター\n" +
                $"- {age}歳の{job}。\n" +
                $"- {background}\n";

            _ai = AIRequestFactory.Create(prompt);
        }

        public async UniTask<IReadOnlyList<string>> RequestSubGoalAsync(CancellationToken token)
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

        public async UniTask<string> RequestLineAsync(RequestLineType type, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            (string content, string sample) instruction = GetContentAndSample(type);

            string prompt =
                $"# 指示内容\n" +
                $"- 自身のキャラクターの設定を基に、{instruction.content}を考えてください。\n" +
                $"- 短い一言で台詞をお願いします。\n" +
                $"'''\n" +
                $"# 出力形式\n" +
                $"- 台詞のみをそのまま出力してください。\n" +
                $"'''\n" +
                $"# 出力例\n" +
                $"- {instruction.sample}\n";
            string response = await _ai.RequestAsync(prompt);

            if (response[0] == '-')
            {
                response = response[1..];
            }

            return response.Trim().Trim('「', '」');
        }

        static (string, string) GetContentAndSample(RequestLineType type)
        {
            if (type == RequestLineType.Entry) return ("登場時の台詞", "頑張るぞ！");
            if (type == RequestLineType.Defeated) return ("敵とのバトルで敗北した際の台詞", "もうだめぽ");
            if (type == RequestLineType.Goal) return ("ゲームをクリアした際の台詞", "ばいばい");
            if (type == RequestLineType.GetTreasureSuccess) return ("宝物を入手した際の台詞", "やったーっ！");
            if (type == RequestLineType.GetTreasureFailure) return ("宝箱の中身が空っぽで残念だった際の台詞", "がっかり");
            if (type == RequestLineType.GetItemSuccess) return ("アイテムを入手した際の台詞", "やったね");
            if (type == RequestLineType.GetItemFailure) return ("アイテムを探したが無かった場合の台詞", "何もないか");
            if (type == RequestLineType.DefeatEnemy) return ("敵を撃破した際の台詞", "勝った！");
            if (type == RequestLineType.Attack) return ("敵を攻撃する際の掛け声", "しゃあっ！");
            if (type == RequestLineType.Damage) return ("攻撃され、ダメージを受けた際の台詞", "くっ！");
            if (type == RequestLineType.Greeting) return ("他の人に声をかける際の挨拶", "ねぇねぇ");

            return default;
        }
    }
}
