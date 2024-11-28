using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EnemyDamageApply : MonoBehaviour
    {
        public string Damage(string id, string weapon, int value, Vector2Int attackerCoords)
        {
            TryGetComponent(out EnemyBlackboard blackboard);

            // ���Ɏ��S���Ă���ꍇ�B
            if (blackboard.IsDefeated) return "Corpse";

            // �_���[�W���o���Đ��B
            if (TryGetComponent(out DamageEffect effect))
            {
                effect.Play(blackboard.Coords, attackerCoords);
            }

            // �̗͂𑀍삷��R���|�[�l���g���H
            blackboard.CurrentHp -= value;
            blackboard.CurrentHp = Mathf.Max(0, blackboard.CurrentHp);

            // ���S�������ǂ����B
            if (blackboard.IsDefeated) return "Defeat";
            else return "Hit";
        }
    }
}
