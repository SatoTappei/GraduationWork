using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class BuffStatusEffect : MonoBehaviour
    {
        Blackboard _blackboard;
        StatusBuffEffect _effect;
        int[] _remainingTurns;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _effect = GetComponent<StatusBuffEffect>();

            // 現状「スピード」と「攻撃力」の2つしかないので、それぞれの残り時間を保持しておくだけで十分。
            _remainingTurns = new int[2];
        }

        public void Apply(string type, float value)
        {
            if (type == "Speed")
            {
                _blackboard.SpeedMagnification = value;
                _remainingTurns[0] = 10; // 適当なターン数
                _effect.Play();
            }
            else if (type == "Attack")
            {
                _blackboard.AttackMagnification = value;
                _remainingTurns[1] = 10; // 適当なターン数
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
                _blackboard.SpeedMagnification = 1.0f;
                _remainingTurns[0] = 0;
            }
            else if (type == "Attack")
            {
                _blackboard.AttackMagnification = 1.0f;
                _remainingTurns[1] = 0;
            }
            else
            {
                Debug.LogWarning($"対応するステータスバフが無い。スペルミス？:{type}");
            }
        }

        public void Tick()
        {
            // 全てのバフの残りターン数を減らす。
            for (int i = 0; i < _remainingTurns.Length; i++)
            {
                _remainingTurns[i]--;
                _remainingTurns[i] = Mathf.Max(0, _remainingTurns[i]);
            }

            // ターン数が0になった場合はバフを解除。
            if (_remainingTurns[0] == 0) Remove("Speed");
            if (_remainingTurns[1] == 0) Remove("Attack");

            // 全てのバフの残りターン数が0になった場合、演出を解除する。
            if (_remainingTurns.Sum() == 0)
            {
                _effect.Stop();
            }
        }
    }
}
