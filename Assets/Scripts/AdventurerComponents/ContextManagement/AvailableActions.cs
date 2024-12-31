using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AvailableActions : MonoBehaviour
    {
        Dictionary<string, float> _actions;
        List<string> _debugView;

        void Awake()
        {
            // スコアは0~1の範囲。0未満の場合、その行動は選択不可能。
            _actions = new Dictionary<string, float>()
            {
                { "MoveNorth", 0.0f },
                { "MoveSouth", 0.0f },
                { "MoveEast", 0.0f },
                { "MoveWest", 0.0f },
                { "MoveToEntrance", -1.0f },
                { "AttackToEnemy", -1.0f },
                { "AttackToAdventurer", -1.0f },
                { "TalkWithAdventurer", -1.0f },
                { "Scavenge", -1.0f },
            };
        }

        public IReadOnlyList<string> GetEntries()
        {
            List<string> entries = new List<string>();
            foreach (KeyValuePair<string, float> e in _actions)
            {
                if (0 <= e.Value)
                {
                    entries.Add($"{e.Key} (Score:{e.Value})");
                }
            }

            // 選択肢が1つ以上あるかチェック。
            if (entries.Count == 0)
            {
                Debug.LogWarning("利用可能な行動の選択肢が1つも無い。");
            }

#if UNITY_EDITOR
            // デバッグ用にインスペクターに表示する内容を更新。
            _debugView ??= new List<string>();
            _debugView.Clear();
            foreach (KeyValuePair<string, float> e in _actions)
            {
                _debugView.Add($"{e.Key}: {e.Value}");
            }
#endif

            return entries;
        }

        public bool SetScore(string action, float score)
        {
            if (_actions.ContainsKey(action))
            {
                _actions[action] = Mathf.Clamp(score, -1.0f, 1.0f);
                return true;
            }
            else
            {
                Debug.LogWarning($"どの行動の選択肢にも該当しない。スペルミス？:{action}");
                return false;
            }
        }
    }
}
