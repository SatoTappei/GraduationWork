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
            GameLog.Add("�V�X�e��", "���҂����`���҂ɒm�b���������B", GameLogColor.Green);

            _isRunning = false;
        }

        void SendToAdventurers(string japanese, string english)
        {
            foreach (Adventurer adventurer in _adventurerSpawner.Spawned)
            {
                if (adventurer == null) continue;

                if (adventurer.TryGetComponent(out TalkReceiver talk))
                {
                    talk.Talk(
                        new BilingualString(japanese, english),
                        "Player", 
                        default,
                        nameof(SendInformationEvent)
                    );
                }
            }
        }
    }
}
