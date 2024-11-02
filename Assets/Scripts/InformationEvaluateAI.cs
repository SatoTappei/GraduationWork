using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game
{
    public class InformationEvaluateAI
    {
        class RequestFormat
        {
            public string Text;
            public string Source;
        }

        AIRequest _ai;

        public InformationEvaluateAI()
        {
            // �L�����N�^�[���𔽉f���Ă��Ȃ��̂ŁA�I�ѕ��͑S�������B
            string prompt =
                $"# Instructions\n" +
                $"- You are a player in a game of dungeon exploration.\n" +
                $"- The combination of information and source is given in Json format.\n" +
                $"- It determines if the information is reliable or not and outputs only the confidence level with a value between 0 and 1.\n" +
                $"'''\n" +
                $"# OutputExample\n" +
                $"- 0.2\n" +
                $"- 1.0\n";
            _ai = AIRequestFactory.Create(prompt);
        }

        public async UniTask<float> EvaluateAsync(SharedInformation information)
        {
            // SharedInformation�^�ɂ�AI�����肷��̂ɕK�v�Ȃ����{��̕��͂ƃX�R�A��񂪊܂܂�Ă���B
            // ���N�G�X�g��p�̌^�ɕK�v�Ȓl���R�s�[���A���̌^�Ń��N�G�X�g����B
            RequestFormat request = new RequestFormat();
            request.Text = information.Text.English;
            request.Source = information.Source;
            
            string result = await _ai.RequestAsync(JsonUtility.ToJson(request));

            // �o�͂��ꂽ���͂̒��ɐ��l�ȊO�̕����񂪊܂܂�Ă���\��������̂ŁA�e���Ă��琔�l�ɕϊ��B
            float score = result
                .Split()
                .Where(s => float.TryParse(s, out float _))
                .Select(t => float.Parse(t))
                .FirstOrDefault();
#if true
            Debug.Log($"���ɑ΂���AI�̕]��: ���:{request.Source}, ���:{request.Text}, �X�R�A:{score}");
#endif
            return score;
        }
    }
}