using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ActionLog : MonoBehaviour
    {
        Queue<string> _log;
        Blackboard _blackboard;

        public IReadOnlyCollection<string> Log => _log;

        void Awake()
        {
            _log = new Queue<string>();
            _blackboard = GetComponent<Blackboard>();
        }

        public void Add(string text)
        {
            _log.Enqueue($"Turn{_blackboard.ElapsedTurn}: {text}");

            // AI�����̍s����I������ۂ̐��x�����Ȃ��璲������B
            // ���̃��O�̏���𑝂₵������ƌ�����AI�̐��x���オ��Ƃ͌���Ȃ��B
            if (_log.Count > 10) _log.Dequeue();
        }
    }
}
