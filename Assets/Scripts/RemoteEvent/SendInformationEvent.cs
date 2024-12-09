using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using AI;

namespace Game
{
    public class SendInformationEvent : MonoBehaviour
    {
        AIClient _ai;
        AdventurerSpawner _adventurerSpawner;
        UiManager _uiManager;

        bool _isRunning;

        void Awake()
        {
            string prompt =
                $"# �w�����e\n" +
                $"- ���͂��^�����܂��B�p��ɖ|�󂵂Ă��������B\n" +
                $"'''\n" +
                $"# �o�͌`��\n" +
                $"- �p��ɖ|�󂵂����݂͂̂����̂܂܏o�͂��Ă��������B\n";
            _ai = new AIClient(prompt);

            AdventurerSpawner.TryFind(out _adventurerSpawner);
            UiManager.TryFind(out _uiManager);
        }

        public void Execute(string text)
        {
            if (_isRunning) return;
            else ExecuteAsync(text, this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask ExecuteAsync(string text, CancellationToken token)
        {
            _isRunning = true;

            string prompt =
                $"# �w�����e\n" +
                $"- ���̕��͂��p��ɖ|�󂵂Ă��������B�u{text}�v\n" +
                $"'''\n" +
                $"# �o�͌`��\n" +
                $"- �p��ɖ|�󂵂����݂͂̂����̂܂܏o�͂��Ă��������B\n";
            string response = await _ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();

            // �`���ґS���ɓ`����B
            if (_adventurerSpawner != null)
            {
                SendToAdventurers(text, response);
            }

            // �C�x���g���s�����O�ɕ\��
            _uiManager.AddLog("<color=#00ff00>���҂����`���҂ɒm�b���������B</color>");

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
