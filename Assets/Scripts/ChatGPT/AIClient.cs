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

        // ��b���i�ނقǕK�v�ȃg�[�N������������B
        // �f�t�H���g�ł̓R���X�g���N�^�ł̃��[���ݒ�1�� + �ŐV��3�񕪂̉�b������n���B
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
            webRequest.SetRequestHeader("Authorization", $"Bearer {APIKey.Value}"); // API�L�[�̔F��
            webRequest.SetRequestHeader("Content-type", "application/json"); // Json�`���Ń��N�G�X�g

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
                    Debug.LogWarning("�o�͂̍ۂɉ��炩�̃G���[���N�����B");
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