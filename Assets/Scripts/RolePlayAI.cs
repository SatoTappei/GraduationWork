using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
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

        public RolePlayAI(IReadOnlyAdventurerContext context)
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
