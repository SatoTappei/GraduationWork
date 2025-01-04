using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AvailableActions : MonoBehaviour
    {
        class Score
        {
            public Score(float value)
            {
                Default = value;
                Current = value;
            }

            public float Default;
            public float Current;
        }

        Dictionary<string, Score> _actions;
        List<string> _debugView;

        void Awake()
        {
            // スコアは0~1の範囲。0未満の場合、その行動は選択不可能。
            _actions = new Dictionary<string, Score>()
            {
                { "MoveNorth", new Score(0.0f) },
                { "MoveSouth", new Score(0.0f) },
                { "MoveEast", new Score(0.0f) },
                { "MoveWest", new Score(0.0f) },
                { "MoveToEntrance", new Score(-1.0f) },
                { "AttackToEnemy", new Score(-1.0f) },
                { "AttackToAdventurer", new Score(-1.0f) },
                { "TalkWithAdventurer", new Score(-1.0f) },
                { "Scavenge", new Score(-1.0f) },
            };
        }

        public IReadOnlyList<string> GetEntries()
        {
            List<string> entries = new List<string>();
            foreach (KeyValuePair<string, Score> e in _actions)
            {
                if (0 <= e.Value.Current)
                {
                    entries.Add($"{e.Key} (Score:{e.Value.Current})");
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
            foreach (KeyValuePair<string, Score> e in _actions)
            {
                _debugView.Add($"{e.Key}: {e.Value.Current}");
            }
#endif

            return entries;
        }

        public bool SetScore(string action, float score)
        {
            if (_actions.ContainsKey(action))
            {
                _actions[action].Current = Mathf.Clamp(score, -1.0f, 1.0f);
                return true;
            }
            else
            {
                Debug.LogWarning($"どの行動の選択肢にも該当しない。スペルミス？:{action}");
                return false;
            }
        }

        public bool ResetScore(string action)
        {
            if (_actions.ContainsKey(action))
            {
                _actions[action].Current = _actions[action].Default;
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
