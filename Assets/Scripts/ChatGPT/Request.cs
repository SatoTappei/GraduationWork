using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [System.Serializable]
    public class Request
    {
        readonly int MaxHistory;

        public string model;
        public List<Message> messages;

        public Request(string model, int maxHistory)
        {
            this.model = model;
            messages = new List<Message>();
            MaxHistory = maxHistory;
        }

        public void AddMessage(string role, string content)
        {
            AddMessage(new Message(role, content));
        }

        public void AddMessage(Message message)
        {
            messages.Add(message);

            // �擪�̓R���X�g���N�^�Ń��[���ݒ������̂�2�Ԗڂ��炪��b�����B
            if (messages.Count > MaxHistory) messages.RemoveAt(1);
        }
    }
}
