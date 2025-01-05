using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.EnemyComponent
{
    public class DamageReceiver : MonoBehaviour, IDamageable
    {
        Enemy _enemy;
        DamageEffect _effect;

        void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _effect = GetComponent<DamageEffect>();
        }

        public string Damage(int value, Vector2Int coords, string effect = "")
        {
            // ���Ɏ��S���Ă���ꍇ�B
            if (_enemy.Status.IsDefeated) return "Corpse";

            // �_���[�W���o���Đ��B
            _effect.Play(coords);

            // �̗͂����炷�B
            _enemy.Status.CurrentHp -= value;

            // ���S�������ǂ�����Ԃ��B
            if (_enemy.Status.IsDefeated) return "Defeat";
            else return "Hit";
        }
    }
}
