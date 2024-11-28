using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class SendInformationEvent : MonoBehaviour
    {
        AIRequest _ai;
        AdventurerSpawner _adventurerSpawner;
        UiManager _uiManager;

        bool _isRunning;

        void Awake()
        {
            string prompt =
                $"# 指示内容\n" +
                $"- 文章が与えられます。英語に翻訳してください。\n" +
                $"'''\n" +
                $"# 出力形式\n" +
                $"- 英語に翻訳した文章のみをそのまま出力してください。\n";
            _ai = AIRequestFactory.Create(prompt);

            _adventurerSpawner = AdventurerSpawner.Find();
            _uiManager = UiManager.Find();
        }

        public void Execute(string text)
        {
            if (_isRunning) return;
            else ExecuteAsync(text, this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask ExecuteAsync(string text, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            _isRunning = true;

            string prompt =
                $"# 指示内容\n" +
                $"- 次の文章を英語に翻訳してください。「{text}」\n" +
                $"'''\n" +
                $"# 出力形式\n" +
                $"- 英語に翻訳した文章のみをそのまま出力してください。\n";
            string response = await _ai.RequestAsync(prompt);

            // 冒険者全員に伝える。
            if (_adventurerSpawner != null)
            {
                SendToAdventurers(text, response);
            }

            // イベント実行をログに表示
            _uiManager.AddLog("<color=#00ff00>何者かが冒険者に知恵を授けた。</color>");

            _isRunning = false;
        }

        void SendToAdventurers(string japanese, string english)
        {
            foreach (Adventurer adventurer in _adventurerSpawner.Spawned)
            {
                if (adventurer == null) continue;
                adventurer.Talk(new BilingualString(japanese, english), nameof(SendInformationEvent), Vector2Int.zero);
            }
        }
    }
}
