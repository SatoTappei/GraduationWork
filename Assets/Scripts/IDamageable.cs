using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IDamageable
    {
        // ���S���_���\�W�������ȂǁA�_���[�W��^�������ʂ�Ԃ���悤�ɂ��Ă����B
        public string Damage(int value, Vector2Int coords, string effect = "");
    }
}
