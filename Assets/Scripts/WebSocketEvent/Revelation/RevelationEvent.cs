using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using AI;

namespace Game
{
    public class RevelationEvent : MonoBehaviour
    {
        AIClient _ai;
        AdventurerSpawner _spawner;

        bool _isRunning;

        void Awake()
        {
            string prompt =
                $"# 指示内容\n" +
                $"- 文章が与えられます。英語に翻訳してください。\n" +
                $"# 出力形式\n" +
                $"- 英語に翻訳した文章のみをそのまま出力してください。\n";
            _ai = new AIClient(prompt);

            _spawner = AdventurerSpawner.Find();
        }

        // ターン数を指定しない場合(-1)はAIの評価から算出する。
        // ユーザーIDを指定しない場合(-1)は全員に伝える。
        public void Execute(string text, int turn = -1, int userID = -1)
        {
            if (_isRunning) return;

            ExecuteAsync(text, turn, userID, this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask ExecuteAsync(string text, int turn, int userID, CancellationToken token)
        {
            _isRunning = true;

#if false
            string prompt =
                $"# 指示内容\n" +
                $"- 次の文章を英語に翻訳してください。「{text}」\n" +
                $"# 出力形式\n" +
                $"- 英語に翻訳した文章のみをそのまま出力してください。\n";
            string response = await _ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();
#else
            // 冒険者名を指定して指示するコメントが来た場合、英訳するとAIが誰を指しているのか理解できない可能性がある。
            string response = text;
            await UniTask.Yield();
#endif

            // 冒険者全員に伝える。
            foreach (Adventurer adventurer in _spawner.Spawned)
            {
                if (adventurer == null) continue;
                if (!(userID == -1 || userID == adventurer.Sheet.UserId)) continue;
                if (!adventurer.TryGetComponent(out TalkReceiver talk)) continue;

                if (turn == -1)
                {
                    BilingualString str = new BilingualString(text, response);
                    talk.Talk(str, "プレイヤー", nameof(RevelationEvent));
                }
                else
                {
                    Information info = new Information(text, response, "Player", 1.0f, turn);
                    talk.Talk(info, nameof(RevelationEvent));
                }
            }

            _isRunning = false;
        }
    }
}
