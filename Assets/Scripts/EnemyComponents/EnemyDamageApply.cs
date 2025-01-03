using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EnemyDamageApply : MonoBehaviour
    {
        public string Damage(int value, Vector2Int coords)
        {
            TryGetComponent(out EnemyBlackboard blackboard);

            // 既に死亡している場合。
            if (blackboard.IsDefeated) return "Corpse";

            // ダメージ演出を再生。
            if (TryGetComponent(out DamageEffect effect))
            {
                effect.Play(coords);
            }

            // 体力を操作するコンポーネント作る？
            blackboard.CurrentHp -= value;
            blackboard.CurrentHp = Mathf.Max(0, blackboard.CurrentHp);

            // 死亡したかどうか。
            if (blackboard.IsDefeated) return "Defeat";
            else return "Hit";
        }
    }
}
