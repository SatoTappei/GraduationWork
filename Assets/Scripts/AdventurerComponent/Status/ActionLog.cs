using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ActionLog
    {
        Queue<string> _log;
        Status _status;

        public ActionLog(Status status)
        {
            _log = new Queue<string>();
            _status = status;
        }

        public IReadOnlyCollection<string> Log => _log;

        public void Add(string text)
        {
            _log.Enqueue($"Turn{_status.ElapsedTurn}: {text}");

            // AI�����̍s����I������ۂ̐��x�����Ȃ��璲������B
            // ���̃��O�̏���𑝂₵������ƌ�����AI�̐��x���オ��Ƃ͌���Ȃ��B
            if (_log.Count > 10) _log.Dequeue();
        }

        public void Delete()
        {
            _log.Clear();
        }
    }
}
