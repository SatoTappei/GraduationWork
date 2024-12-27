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
            // 他の冒険者に伝える内容を判断する基準はAI任せなので、ヒントより挨拶を優先してしまうことがある。
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
            // 全ての情報が空文字の場合はAIが正常に判断できない可能性がある。
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

            // 出力された文章の中に数値以外の文字列が含まれている可能性があるので、弾いてから数値に変換。
            int number = result
                .Split()
                .Where(s => int.TryParse(s, out int _))
                .Select(t => int.Parse(t))
                .FirstOrDefault();
            
            return information[number];
        }
    }
}
