using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class SubGoal : MonoBehaviour
    {
        // �������BUI�ւ̕\���̑��AAI�ւ̃��N�G�X�g�ɂ��g���̂ŁA���{��Ɖp���̗������K�v�B
        public abstract BilingualString Description { get; }

        // ���������𖞂��������ǂ����B
        public abstract bool IsCompleted();
    }
}
