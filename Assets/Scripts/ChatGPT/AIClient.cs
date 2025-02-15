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
        public AIClient(string prompt, string model = "gpt-4o-mini", int maxHistory = 7)
        {
            _request = new Request(model, maxHistory);
            _request.AddMessage("system", prompt);
        }

        public async UniTask<string> RequestAsync(string prompt, CancellationToken token)
        {
            _request.AddMessage("user", prompt);

            using UnityWebRequest webRequest = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST");
            byte[] uploadData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(_request));
            webRequest.uploadHandler = new UploadHandlerRaw(uploadData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.timeout = 5;
            webRequest.SetRequestHeader("Authorization", $"Bearer {APIKey.Value}"); // APIキーの認証
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

                if (response.choices == null || response.choices.Length == 0)
                {
                    Debug.LogWarning("出力の際に何らかのエラーが起きた。");
                    return string.Empty;
                }
                else
                {
                    Message msg = response.choices[0].message;
                    _request.AddMessage(msg);

                    return msg.content;
                }
            }
        }
    }
}