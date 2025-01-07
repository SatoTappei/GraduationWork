using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class BuffStatusEffect : StatusEffect
    {
        Adventurer _adventurer;
        StatusBuffEffect _effect;
        int[] _turns;

        public override string Description => "力が漲っている。";
        public override bool IsValid => _turns.Sum() > 0;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _effect = GetComponent<StatusBuffEffect>();

            // 現状「スピード」と「攻撃力」の2つしかないので、それぞれの残り時間を保持しておくだけで十分。
            _turns = new int[2];
        }

        public void Set(string type, float value)
        {
            if (type == "Speed")
            {
                _adventurer.Status.SpeedMagnification = value;
                _turns[0] = 10; // 適当なターン数
                _effect.Play();
            }
            else if (type == "Attack")
            {
                _adventurer.Status.AttackMagnification = value;
                _turns[1] = 10; // 適当なターン数
                _effect.Play();
            }
            else
            {
                Debug.LogWarning($"対応するステータスバフが無い。スペルミス？:{type}");
            }
        }

        public void Remove(string type)
        {
            if (type == "Speed")
            {
                _adventurer.Status.SpeedMagnification = 1.0f;
                _turns[0] = 0;
            }
            else if (type == "Attack")
            {
                _adventurer.Status.AttackMagnification = 1.0f;
                _turns[1] = 0;
            }
            else
            {
                Debug.LogWarning($"対応するステータスバフが無い。スペルミス？:{type}");
            }
        }

        public override void Apply()
        {
            // 全てのバフの残りターン数を減らす。
            for (int i = 0; i < _turns.Length; i++)
            {
                _turns[i]--;
                _turns[i] = Mathf.Max(0, _turns[i]);
            }

            // ターン数が0になった場合はバフを解除。
            if (_turns[0] == 0) Remove("Speed");
            if (_turns[1] == 0) Remove("Attack");

            // 全てのバフの残りターン数が0になった場合、演出を解除する。
            if (_turns.Sum() == 0)
            {
                _effect.Stop();
            }
        }
    }
}
