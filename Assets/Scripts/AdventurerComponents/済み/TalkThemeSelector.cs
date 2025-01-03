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

        InformationStock _informationStock;
        AIClient _ai;

        public BilingualString Selected { get; private set; }

        void Awake()
        {
            _informationStock = GetComponent<InformationStock>();

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
            if (_informationStock.Stock == null || _informationStock.Stock.Count == 0)
            {
                Selected = null;
                return;
            }
            
            // AI�Ƀ��N�G�X�g����p�̃t�H�[�}�b�g�ɕϊ��B
            // �ێ����Ă���e���̉p���ɑΉ�����ԍ����ӂ�B
            Choice[] choices = new Choice[_informationStock.Stock.Count];
            for (int i = 0; i < choices.Length; i++)
            {
                Information info = _informationStock.Stock[i];

                if (info == null) continue;
                else if (info.Text==null)continue;
                else if (!info.IsShared) continue;

                choices[i] = new Choice();
                choices[i].Text = info.Text.English;
                choices[i].Number = i;
            }
            
            RequestFormat request = new RequestFormat();
            request.Choices = choices;
            string result = await _ai.RequestAsync(JsonUtility.ToJson(request), token);
            token.ThrowIfCancellationRequested();

            // �o�͂��ꂽ���͂̒��ɐ��l�ȊO�̕����񂪊܂܂�Ă���\��������̂ŁA�e���Ă��琔�l�ɕϊ��B
            int index = result
                .Split()
                .Where(s => int.TryParse(s, out int _))
                .Select(t => int.Parse(t))
                .FirstOrDefault();

            // ���N�G�X�g����p�̃t�H�[�}�b�g�ɂ͉p�������f�[�^�������B
            // ���Ԃ͌��̏��ƈꏏ�Ȃ̂ŁA���̏���ԍ��Ŏw�肷��B
            if (0 <= index && index < _informationStock.Stock.Count)
            {
                Selected = _informationStock.Stock[index].Text;
            }
            else
            {
                Debug.LogWarning($"�Ή������񂪖����̂ŉ�b���e��I���o���Ȃ��B: {index}");
            }
        }
    }
}
