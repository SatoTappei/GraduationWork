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

            // �擪�̓R���X�g���N�^�Ń��[���ݒ������̂�2�Ԗڂ��炪��b�����B
            if (messages.Count > MaxHistoryLength) messages.RemoveAt(1);
        }
    }
}
