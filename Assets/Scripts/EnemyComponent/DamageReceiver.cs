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
            // 既に死亡している場合。
            if (_enemy.Status.IsDefeated) return "Corpse";

            // ダメージ演出を再生。
            _effect.Play(coords);

            // 体力を減らす。
            _enemy.Status.CurrentHp -= value;

            // 死亡したかどうかを返す。
            if (_enemy.Status.IsDefeated) return "Defeat";
            else return "Hit";
        }
    }
}
