using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DamageReceiver : MonoBehaviour
    {
        Blackboard _blackboard;
        DamageEffect _effect;
        MadnessStatusEffect _madness;
        LineDisplayer _line;
        StatusBarBinder _statusBar;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _effect = GetComponent<DamageEffect>();
            _madness = GetComponent<MadnessStatusEffect>();
            _line = GetComponent<LineDisplayer>();
            _statusBar = GetComponent<StatusBarBinder>();
        }

        public string Damage(int value, Vector2Int coords, string effect)
        {
            // ���Ɏ��S���Ă���ꍇ�B
            if (_blackboard.IsDefeated) return "Corpse";

            // �_���[�W���o���Đ��B
            _effect.Play(coords);

            // ���C��t�^����ꍇ�B
            if (effect == "Madness") _madness.Apply();

            // �̗͂����炷�B
            _blackboard.CurrentHp -= value;
            _blackboard.CurrentHp = Mathf.Max(0, _blackboard.CurrentHp);

            // �_���[�W���󂯂��ۂ̑䎌�B
            _line.ShowLine(RequestLineType.Damage);

            // UI�ɔ��f�B
            _statusBar.Apply();

            // ���S�������ǂ�����Ԃ��B
            if (_blackboard.IsDefeated) return "Defeat";
            else return "Hit";
        }
    }
}
