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

            // 既に死亡している場合。
            if (blackboard.IsDefeated) return "Corpse";

            // ダメージ演出を再生。
            if (TryGetComponent(out DamageEffect effect))
            {
                effect.Play(blackboard.Coords, attackerCoords);
            }

            // 狂気を付与する場合。
            if (weapon == "Madness" && TryGetComponent(out MadnessApply madness))
            {
                madness.Apply();
            }

            // 体力を操作するコンポーネント作る？
            blackboard.CurrentHp -= value;
            blackboard.CurrentHp = Mathf.Max(0, blackboard.CurrentHp);

            // ダメージを受けた際の台詞。
            if (TryGetComponent(out LineApply line))
            {
                line.ShowLine(RequestLineType.Damage);
            }

            // UIに反映。
            if (TryGetComponent(out StatusBarApply statusBar))
            {
                statusBar.Apply();
            }

            // 死亡したかどうか。
            if (blackboard.IsDefeated) return "Defeat";
            else return "Hit";
        }
    }
}
