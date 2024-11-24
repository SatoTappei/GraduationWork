using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // �e�T�u�S�[���̃N���X�͂��̃N���X���p������B
    // AI���I�񂾃T�u�S�[���ɑΉ�����N���X��AddComponent����z��B
    public abstract class SubGoal : MonoBehaviour
    {
        public abstract BilingualString Text { get; }

        public abstract bool IsCompleted();
        public virtual IEnumerable<string> GetAdditionalActions() { yield break; }
    }
}
