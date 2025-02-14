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
            // 既に死亡している場合。
            if (_adventurer.Status.IsDefeated) return "Corpse";

            // ダメージ演出を再生。
            if (value >= 1) _effect.Play(coords);

            // 狂気を付与する場合。
            if (effect == "Madness") _madness.Set();

            // 体力を減らす。
            _adventurer.Status.CurrentHp -= value;

            // ダメージを受けた際の台詞。
            _line.Show(RequestLineType.Damage);

            // UIに反映。
            _statusBar.Apply();

            // 死亡したかどうかを返す。
            if (_adventurer.Status.IsDefeated) return "Defeat";
            else return "Hit";
        }
    }
}
