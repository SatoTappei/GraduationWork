using Cysharp.Threading.Tasks;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace AI
{
    public class AIClient
    {
        Request _request;

        // 会話が進むほど必要なトークン数が増える。
        // デフォルトではコンストラクタでのロール設定1回 + 最新の3回分の会話履歴を渡す。
        public AIClient(string prompt, int maxHistoryLength = 7)
        {
            _request = new Request("gpt-4o-mini", maxHistoryLength);
            _request.AddMessage("system", prompt);
        }

        public async UniTask<string> RequestAsync(string prompt, CancellationToken token)
        {
            const string Key = "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";

            _request.AddMessage("user", prompt);

            using UnityWebRequest webRequest = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST");
            byte[] uploadData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(_request));
            webRequest.uploadHandler = new UploadHandlerRaw(uploadData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.timeout = 5;
            webRequest.SetRequestHeader("Authorization", $"Bearer {Key}");   // APIキーの認証
            webRequest.SetRequestHeader("Content-type", "application/json"); // Json形式でリクエスト

            try
            {
                await webRequest.SendWebRequest().WithCancellation(token);
            }
            catch (UnityWebRequestException e)
            {
                Debug.LogError(e.Message);
                return e.Message;
            }

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(webRequest.result);
                return webRequest.result.ToString();
            }
            else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.result);
                return webRequest.result.ToString();
            }
            else
            {
                string downloadText = webRequest.downloadHandler.text;
                Response response = JsonUtility.FromJson<Response>(downloadText);
                Message responseMessage = response.choices[0].message;

                _request.Add(responseMessage);

                return responseMessage.content;
            }
        }
    }
}
