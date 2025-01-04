using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class StatusEffect : MonoBehaviour
    {
        // ���ʂ𔽉f����B
        public abstract void Apply();

        // UI�ɕ\�������Ԉُ�̐������B
        public abstract string Description { get; }

        // ��Ԉُ킪���ݗL�����ǂ����𔻒肷��B
        public abstract bool IsValid { get; }
    }
}
