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
                $"# �w�����e\n" +
                $"- ���͂��^�����܂��B�p��ɖ|�󂵂Ă��������B\n" +
                $"# �o�͌`��\n" +
                $"- �p��ɖ|�󂵂����݂͂̂����̂܂܏o�͂��Ă��������B\n";
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
                $"# �w�����e\n" +
                $"- ���̕��͂��p��ɖ|�󂵂Ă��������B�u{text}�v\n" +
                $"# �o�͌`��\n" +
                $"- �p��ɖ|�󂵂����݂͂̂����̂܂܏o�͂��Ă��������B\n";
            string response = await _ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();

            // �`���ґS���ɓ`����B
            foreach (Adventurer adventurer in _spawner.Spawned)
            {
                if (adventurer == null) continue;
                if (!(userID == -1 || userID == adventurer.Sheet.UserId)) continue;
                
                if (adventurer.TryGetComponent(out TalkReceiver talk))
                {
                    talk.Talk(
                        new BilingualString(text, response),
                        "�v���C���[",
                        default,
                        nameof(RevelationEvent)
                    );
                }
            }

            _isRunning = false;
        }
    }
}
