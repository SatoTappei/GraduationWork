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

        // �^�[�������w�肵�Ȃ��ꍇ(-1)��AI�̕]������Z�o����B
        // ���[�U�[ID���w�肵�Ȃ��ꍇ(-1)�͑S���ɓ`����B
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
                $"# �w�����e\n" +
                $"- ���̕��͂��p��ɖ|�󂵂Ă��������B�u{text}�v\n" +
                $"# �o�͌`��\n" +
                $"- �p��ɖ|�󂵂����݂͂̂����̂܂܏o�͂��Ă��������B\n";
            string response = await _ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();
#else
            // �`���Җ����w�肵�Ďw������R�����g�������ꍇ�A�p�󂷂��AI���N���w���Ă���̂������ł��Ȃ��\��������B
            string response = text;
            await UniTask.Yield();
#endif

            // �`���ґS���ɓ`����B
            foreach (Adventurer adventurer in _spawner.Spawned)
            {
                if (adventurer == null) continue;
                if (!(userID == -1 || userID == adventurer.Sheet.UserId)) continue;
                if (!adventurer.TryGetComponent(out TalkReceiver talk)) continue;

                if (turn == -1)
                {
                    BilingualString str = new BilingualString(text, response);
                    talk.Talk(str, "�v���C���[", nameof(RevelationEvent));
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
