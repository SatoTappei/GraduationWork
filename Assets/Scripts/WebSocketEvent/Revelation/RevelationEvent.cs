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

        public void Execute(string text, int userID = -1)
        {
            if (_isRunning) return;

            ExecuteAsync(text, userID, this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask ExecuteAsync(string text, int userID, CancellationToken token)
        {
            _isRunning = true;

            string prompt =
                $"# 指示内容\n" +
                $"- 次の文章を英語に翻訳してください。「{text}」\n" +
                $"# 出力形式\n" +
                $"- 英語に翻訳した文章のみをそのまま出力してください。\n";
            string response = await _ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();

            // 冒険者全員に伝える。
            foreach (Adventurer adventurer in _spawner.Spawned)
            {
                if (adventurer == null) continue;
                if (!(userID == -1 || userID == adventurer.Sheet.UserId)) continue;
                
                if (adventurer.TryGetComponent(out TalkReceiver talk))
                {
                    talk.Talk(
                        new BilingualString(text, response),
                        "プレイヤー",
                        default,
                        nameof(RevelationEvent)
                    );
                }
            }

            _isRunning = false;
        }
    }
}
