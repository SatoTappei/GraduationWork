using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ActionLog
    {
        Queue<string> _log;

        public ActionLog()
        {
            _log = new Queue<string>();
        }

        public IReadOnlyCollection<string> Log => _log;

        public void Add(string text)
        {
            _log.Enqueue(text);

            // AI�����̍s����I������ۂ̐��x�����Ȃ��璲������B
            // ���̃��O�̏���𑝂₵������ƌ�����AI�̐��x���オ��Ƃ͌���Ȃ��B
            if (_log.Count > 3) _log.Dequeue();
        }

        public void Delete()
        {
            _log.Clear();
        }
    }
}
