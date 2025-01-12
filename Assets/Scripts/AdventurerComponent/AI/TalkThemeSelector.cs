using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using AI;

namespace Game
{
    public class TalkThemeSelector : MonoBehaviour
    {
        [System.Serializable]
        class RequestFormat
        {
            public Choice[] Choices;
        }

        [System.Serializable]
        class Choice
        {
            public string Text;
            public int Number;
        }

        HoldInformation _informationStock;
        AIClient _ai;

        public BilingualString Selected { get; private set; }

        void Awake()
        {
            _informationStock = GetComponent<HoldInformation>();

            // ���̖`���҂ɓ`������e�𔻒f������AI�C���Ȃ̂ŁA�q���g��舥�A��D�悵�Ă��܂����Ƃ�����B
            string prompt =
                $"# Instructions\n" +
                $"- You are a player in a game of dungeon exploration.\n" +
                $"- You need to tell other players what you know.\n" +
                $"- I leave it to you to decide which information to tell.\n" +
                $"- A set of text and a number is given.\n" +
                $"# OutputFormat\n" +
                $"- Output only the number of the selected text.";
            _ai = new AIClient(prompt);
        }

        public async UniTask SelectAsync(CancellationToken token)
        {
            // �ێ����Ă����񂪖����ꍇ�A�`������������B
            if (_informationStock.Information == null || _informationStock.Information.Count == 0)
            {
                Selected = null;
                return;
            }
            
            // AI�Ƀ��N�G�X�g����p�̃t�H�[�}�b�g�ɕϊ��B
            // �ێ����Ă���e���̉p���ɑΉ�����ԍ����ӂ�B
            Choice[] choices = new Choice[_informationStock.Information.Count];
            for (int i = 0; i < choices.Length; i++)
            {
                Information info = _informationStock.Information[i];

                if (info == null) continue;
                else if (info.Text==null)continue;
                else if (!info.IsShared) continue;

                choices[i] = new Choice();
                choices[i].Text = info.Text.English;
                choices[i].Number = i;
            }

#if true
            RequestFormat request = new RequestFormat();
            request.Choices = choices;
            string json = JsonUtility.ToJson(request, prettyPrint: true);
            string result = await _ai.RequestAsync(json, token);
            token.ThrowIfCancellationRequested();
#else
            await UniTask.Yield(cancellationToken:token);
            // �f�o�b�O�p�B�I����AI�Ƀ��N�G�X�g�����A�����_���őI������B
            Debug.Log("API�ɉ�b���e�̑I�������N�G�X�g���Ă��Ȃ���ԂŎ��s���B");
            string result = Random.Range(0, choices.Length).ToString();
#endif
            // �o�͂��ꂽ���͂̒��ɐ��l�ȊO�̕����񂪊܂܂�Ă���\��������̂ŁA�e���Ă��琔�l�ɕϊ��B
            int index = result
                .Split()
                .Where(s => int.TryParse(s, out int _))
                .Select(t => int.Parse(t))
                .FirstOrDefault();

            // ���N�G�X�g����p�̃t�H�[�}�b�g�ɂ͉p�������f�[�^�������B
            // ���Ԃ͌��̏��ƈꏏ�Ȃ̂ŁA���̏���ԍ��Ŏw�肷��B
            if (0 <= index && index < _informationStock.Information.Count)
            {
                Selected = _informationStock.Information[index].Text;
            }
            else
            {
                Debug.LogWarning($"�Ή������񂪖����̂ŉ�b���e��I���o���Ȃ��B: {index}");
            }
        }
    }
}
