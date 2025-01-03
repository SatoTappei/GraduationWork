using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using AI;

namespace Game
{
    public class InformationEvaluator
    {
        [System.Serializable]
        class RequestFormat
        {
            public string Text;
            public string Source;
        }

        AIClient _ai;

        public InformationEvaluator()
        {
            // �L�����N�^�[���𔽉f���Ă��Ȃ��̂ŁA�I�ѕ��͑S�������B
            string prompt =
                $"# Instructions\n" +
                $"- You are a player in a game of dungeon exploration.\n" +
                $"- Information and the source of that information are given.\n" +
                $"- Determine the reliability of the information.\n" +
                $"# OutputFormat\n" +
                $"- Output the confidence level of the information as a number between 0 and 1.\n" +
                $"# OutputExample\n" +
                $"- 0.2\n" +
                $"- 1.0\n";

            _ai = new AIClient(prompt);
        }

        public async UniTask<float> EvaluateAsync(Information information, CancellationToken token)
        {
            if (information == null)
            {
                Debug.LogWarning("�]�����悤�Ƃ������null");
                return 0;
            }
            else if (information.Text == null)
            {
                Debug.LogWarning("�]�����悤�Ƃ������̕��͂�null");
                return 0;
            }

            // SharedInformation�^�ɂ�AI�����肷��̂ɕK�v�Ȃ����{��̕��͂ƃX�R�A��񂪊܂܂�Ă���B
            // ���N�G�X�g��p�̌^�ɕK�v�Ȓl���R�s�[���A���̌^�Ń��N�G�X�g����B
            RequestFormat request = new RequestFormat();
            request.Text = information.Text.English;
            request.Source = information.Source;
            
            string result = await _ai.RequestAsync(JsonUtility.ToJson(request), token);
            token.ThrowIfCancellationRequested();

            // �o�͂��ꂽ���͂̒��ɐ��l�ȊO�̕����񂪊܂܂�Ă���\��������̂ŁA�e���Ă��琔�l�ɕϊ��B
            float score = result
                .Split()
                .Where(s => float.TryParse(s, out float _))
                .Select(t => float.Parse(t))
                .FirstOrDefault();
#if false
            Debug.Log($"���ɑ΂���AI�̕]��: ���:{request.Source}, ���:{request.Text}, �X�R�A:{score}");
#endif
            return score;
        }
    }
}