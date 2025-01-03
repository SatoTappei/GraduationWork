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
            // 既に死亡している場合。
            if (_blackboard.IsDefeated) return "Corpse";

            // ダメージ演出を再生。
            _effect.Play(coords);

            // 狂気を付与する場合。
            if (effect == "Madness") _madness.Apply();

            // 体力を減らす。
            _blackboard.CurrentHp -= value;
            _blackboard.CurrentHp = Mathf.Max(0, _blackboard.CurrentHp);

            // ダメージを受けた際の台詞。
            _line.ShowLine(RequestLineType.Damage);

            // UIに反映。
            _statusBar.Apply();

            // 死亡したかどうかを返す。
            if (_blackboard.IsDefeated) return "Defeat";
            else return "Hit";
        }
    }
}
