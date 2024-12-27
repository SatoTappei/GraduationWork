using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DamageApply : MonoBehaviour
    {
        public string Damage(string id, string weapon, int value, Vector2Int attackerCoords)
        {
            TryGetComponent(out Blackboard blackboard);

            // ���Ɏ��S���Ă���ꍇ�B
            if (blackboard.IsDefeated) return "Corpse";

            // �_���[�W���o���Đ��B
            if (TryGetComponent(out DamageEffect effect))
            {
                effect.Play(blackboard.Coords, attackerCoords);
            }

            // ���C��t�^����ꍇ�B
            if (weapon == "Madness" && TryGetComponent(out MadnessApply madness))
            {
                madness.Apply();
            }

            // �̗͂𑀍삷��R���|�[�l���g���H
            blackboard.CurrentHp -= value;
            blackboard.CurrentHp = Mathf.Max(0, blackboard.CurrentHp);

            // �_���[�W���󂯂��ۂ̑䎌�B
            if (TryGetComponent(out LineApply line))
            {
                line.ShowLine(RequestLineType.Damage);
            }

            // UI�ɔ��f�B
            if (TryGetComponent(out StatusBarApply statusBar))
            {
                statusBar.Apply();
            }

            // ���S�������ǂ����B
            if (blackboard.IsDefeated) return "Defeat";
            else return "Hit";
        }
    }
}
