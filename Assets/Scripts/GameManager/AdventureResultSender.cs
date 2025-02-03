using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Unused
{
    // 未使用。管理方法がGASではなくなった。
    public static class AdventureResultSender
    {
        public class ProcessStatus
        {
            public bool isProcessing;
        }

        public static async UniTask SendAsync(IReadOnlyDictionary<Adventurer, string> adventureResults, CancellationToken token)
        {
            // 卒業制作_冒険者データ のGASのデプロイしたURL。
            const string URL = "https://script.google.com/macros/s/AKfycby6-OPwF24uUzoU9e9XFBRBZybEEZkMCKuayL8sjr39pooCzvYQaQzBQi2RIqpR7lu-QQ/exec";

            // GAS側で値を取得できるようキーに対する値を設定。
            string format = AdventureResultPostFormat.Convert(adventureResults);
            WWWForm form = new WWWForm();
            form.AddField("results", format);

            using UnityWebRequest request = UnityWebRequest.Post($"{URL}?type=result", form);
            await request.SendWebRequest().WithCancellation(token);

            // スプレッドシートに書き込みが完了するまで待つ。
            bool isProcessing = true;
            while (isProcessing && !token.IsCancellationRequested)
            {
                using UnityWebRequest getRequest = UnityWebRequest.Get($"{URL}?type=completion");
                await getRequest.SendWebRequest().WithCancellation(token);

                string json = getRequest.downloadHandler.text;
                isProcessing = JsonUtility.FromJson<ProcessStatus>(json).isProcessing;
            }

            Debug.Log("冒険の結果をスプレッドシートに書き込み完了。");
        }
    }

    public static class AdventureResultPostFormat
    {
        [System.Serializable]
        class Format
        {
            public Format(IReadOnlyDictionary<Adventurer, string> adventureResults)
            {
                // 冒険者の名前と冒険結果をセットで保持する。
                // 同じ名前の冒険者が複数存在する場合、Postした先で区別できないので注意。
                List<Result> results = new List<Result>(adventureResults.Count);
                foreach (KeyValuePair<Adventurer, string> result in adventureResults)
                {
                    string adventurerName = result.Key.Sheet.FullName;
                    string resultContents = result.Value;
                    results.Add(new Result(adventurerName, resultContents));
                }

                Results = results.ToArray();
            }

            public Result[] Results;
        }

        [System.Serializable]
        class Result
        {
            public Result(string name, string contents)
            {
                Name = name;
                Contents = contents;
            }

            public string Name;
            public string Contents;
        }

        public static string Convert(IReadOnlyDictionary<Adventurer, string> adventureResults)
        {
            Format format = new Format(adventureResults);
            return JsonUtility.ToJson(format);
        }
    }
}
