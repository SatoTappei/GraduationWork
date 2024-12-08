using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [System.Serializable]
    public class Request
    {
        readonly int MaxHistoryLength;

        public string model;
        public List<Message> messages;

        public Request(string model, int maxHistoryLength)
        {
            this.model = model;
            messages = new List<Message>();
            MaxHistoryLength = maxHistoryLength;
        }

        public void AddMessage(string role, string content)
        {
            Add(new Message(role, content));
        }

        public void Add(Message message)
        {
            messages.Add(message);

            // 先頭はコンストラクタでロール設定を入れるので2番目からが会話履歴。
            if (messages.Count > MaxHistoryLength) messages.RemoveAt(1);
        }
    }
}
