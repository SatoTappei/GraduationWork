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

            // 他の冒険者に伝える内容を判断する基準はAI任せなので、ヒントより挨拶を優先してしまうことがある。
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
            // 保持している情報が無い場合、伝える情報も無い。
            if (_informationStock.Information == null || _informationStock.Information.Count == 0)
            {
                Selected = null;
                return;
            }
            
            // AIにリクエストする用のフォーマットに変換。
            // 保持している各情報の英文に対応する番号をふる。
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
            string result = await _ai.RequestAsync(JsonUtility.ToJson(request), token);
            token.ThrowIfCancellationRequested();
#else
            await UniTask.Yield(cancellationToken:token);
            // デバッグ用。選択をAIにリクエストせず、ランダムで選択する。
            Debug.Log("APIに会話内容の選択をリクエストしていない状態で実行中。");
            string result = Random.Range(0, choices.Length).ToString();
#endif
            // 出力された文章の中に数値以外の文字列が含まれている可能性があるので、弾いてから数値に変換。
            int index = result
                .Split()
                .Where(s => int.TryParse(s, out int _))
                .Select(t => int.Parse(t))
                .FirstOrDefault();

            // リクエストする用のフォーマットには英文しかデータが無い。
            // 順番は元の情報と一緒なので、元の情報を番号で指定する。
            if (0 <= index && index < _informationStock.Information.Count)
            {
                Selected = _informationStock.Information[index].Text;
            }
            else
            {
                Debug.LogWarning($"対応する情報が無いので会話内容を選択出来ない。: {index}");
            }
        }
    }
}
