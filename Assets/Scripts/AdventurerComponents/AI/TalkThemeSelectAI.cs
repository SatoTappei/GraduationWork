using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using AI;

namespace Game
{
    public class TalkThemeSelectAI : MonoBehaviour
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

        AIClient _ai;

        void Awake()
        {
            // ���̖`���҂ɓ`������e�𔻒f������AI�C���Ȃ̂ŁA�q���g��舥�A��D�悵�Ă��܂����Ƃ�����B
            string prompt =
                $"# Instructions\n" +
                $"- You are a player in a game of dungeon exploration.\n" +
                $"- You need to tell other players what you know, so choose which information you want to tell them.\n" +
                $"- Input is given in Json format.\n" +
                $"- Several options will be presented as text with corresponding numbers. Select the option that best aligns with your choice and return only the number of the selected option.";
            _ai = new AIClient(prompt);
        }

        public async UniTask<Information> SelectAsync(IReadOnlyList<Information> information, CancellationToken token)
        {
            // �S�Ă̏�񂪋󕶎��̏ꍇ��AI������ɔ��f�ł��Ȃ��\��������B
            bool isEmpty = true;
            foreach (Information info in information)
            {
                if (info.Text.English != string.Empty && info.IsShared)
                {
                    isEmpty = false;
                    break;
                }
            }
            
            if (isEmpty) return null;
            
            Choice[] choices = new Choice[information.Count];
            for (int i = 0; i < choices.Length; i++)
            {
                if (information[i] == null || !information[i].IsShared) continue;

                choices[i] = new Choice();
                choices[i].Text = information[i].Text.English;
                choices[i].Number = i;
            }
            
            RequestFormat request = new RequestFormat();
            request.Choices = choices;
            string result = await _ai.RequestAsync(JsonUtility.ToJson(request), token);
            token.ThrowIfCancellationRequested();

            // �o�͂��ꂽ���͂̒��ɐ��l�ȊO�̕����񂪊܂܂�Ă���\��������̂ŁA�e���Ă��琔�l�ɕϊ��B
            int number = result
                .Split()
                .Where(s => int.TryParse(s, out int _))
                .Select(t => int.Parse(t))
                .FirstOrDefault();
            
            return information[number];
        }
    }
}
