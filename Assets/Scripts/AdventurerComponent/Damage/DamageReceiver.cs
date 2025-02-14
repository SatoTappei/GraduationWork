using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DamageReceiver : MonoBehaviour, IDamageable
    {
        Adventurer _adventurer;
        DamageEffect _effect;
        MadnessStatusEffect _madness;
        LineDisplayer _line;
        StatusBarBinder _statusBar;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _effect = GetComponent<DamageEffect>();
            _madness = GetComponent<MadnessStatusEffect>();
            _line = GetComponent<LineDisplayer>();
            _statusBar = GetComponent<StatusBarBinder>();
        }

        public string Damage(int value, Vector2Int coords, string effect = "")
        {
            // ���Ɏ��S���Ă���ꍇ�B
            if (_adventurer.Status.IsDefeated) return "Corpse";

            // �_���[�W���o���Đ��B
            if (value >= 1) _effect.Play(coords);

            // ���C��t�^����ꍇ�B
            if (effect == "Madness") _madness.Set();

            // �̗͂����炷�B
            _adventurer.Status.CurrentHp -= value;

            // �_���[�W���󂯂��ۂ̑䎌�B
            _line.Show(RequestLineType.Damage);

            // UI�ɔ��f�B
            _statusBar.Apply();

            // ���S�������ǂ�����Ԃ��B
            if (_adventurer.Status.IsDefeated) return "Defeat";
            else return "Hit";
        }
    }
}
