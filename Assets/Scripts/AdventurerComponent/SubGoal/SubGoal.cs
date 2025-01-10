using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class SubGoal : MonoBehaviour
    {
        public enum State
        {
            Pending,   // ���s�҂��B
            Running,   // ���s���B
            Completed, // ���������𖞂������B
            Failed,    // ���炩�̌����Ŏ��s�����B
        }

        // UI�ւ̕\���̑��AAI�ւ̃��N�G�X�g�ɂ��g���̂ŁA���{��Ɖp���̗������K�v�B
        public abstract BilingualString Description { get; }

        public abstract State Check();
    }
}
