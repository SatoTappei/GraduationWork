using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class AIMessage
{
    public string role;
    public string content;
}

[System.Serializable]
public class AIResponse
{
    public string id;
    public string @object;
    public int created;
    public string model;
    public Choice[] choices;
    public Usage usage;

    [System.Serializable]
    public class Choice
    {
        public int index;
        public AIMessage message;
        public string finish_reason;
    }

    [System.Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
}

[System.Serializable]
public class AIConfig
{
    public string model;
    public List<AIMessage> messages;
}

public class AIRequest
{
    List<AIMessage> _messages;
    AIConfig _config;
    string _key;
    int _capacity;

    // ��b���i�ނقǕK�v�ȃg�[�N������������B
    // �f�t�H���g�ł̓R���X�g���N�^�ł̃��[���ݒ�1�� + �ŐV��3�񕪂̉�b������n���B
    public AIRequest(string key, string content, int capacity = 7)
    {
        _messages = new List<AIMessage>();
        _config = new AIConfig { model = "gpt-4o-mini", messages = _messages };
        _key = key;
        _capacity = capacity;

        CreateNewRequestMessage("system", content);
    }

    public async UniTask<string> RequestAsync(string message)
    {
        CreateNewRequestMessage("user", message);

        using UnityWebRequest request = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST");
        request.uploadHandler = new UploadHandlerRaw(GetConfigBytes());
        request.downloadHandler = new DownloadHandlerBuffer();
        request.timeout = 5;
        request.SetRequestHeader("Authorization", $"Bearer {_key}"); // API�L�[�̔F��
        request.SetRequestHeader("Content-type", "application/json"); // Json�`���Ń��N�G�X�g

        await request.SendWebRequest();

        if (IsSuccess(request.result))
        {
            AIMessage response = GetResponseMessage(request);
            AddRequestMessage(response);
            return response.content;
        }
        else
        {
            throw new UnityWebRequestException(request);
        }
    }

    void CreateNewRequestMessage(string role, string content)
    {
        AIMessage msg = new AIMessage { role = role, content = content };
        AddRequestMessage(msg);
    }

    void AddRequestMessage(AIMessage msg)
    {
        _messages ??= new List<AIMessage>();
        _messages.Add(msg);

        // �擪�̓R���X�g���N�^�Ń��[���ݒ����ꂽ�̂�2�Ԗڂ��炪��b�����B
        if (_messages.Count > _capacity)
        {
            _messages.RemoveAt(1);
        }
    }

    byte[] GetConfigBytes()
    {
        string json = JsonUtility.ToJson(_config);
        return Encoding.UTF8.GetBytes(json);
    }

    static bool IsSuccess(UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.ConnectionError) return false;
        if (result == UnityWebRequest.Result.ProtocolError) return false;
        else return true;
    }

    static AIMessage GetResponseMessage(UnityWebRequest request)
    {
        string text = request.downloadHandler.text;
        AIResponse response = JsonUtility.FromJson<AIResponse>(text);
        return response.choices[0].message;
    }
}
