using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AvailableActions : MonoBehaviour
    {
        public class Score
        {
            public Score(float value)
            {
                Default = value;
                Current = value;
                Weight = 1.0f;
            }

            public float Default;
            public float Current;
            public float Weight;

            public float Total => Current * Weight;
        }

        Dictionary<string, Score> _actions;
        List<string> _debugView;

        void Awake()
        {
            // スコアは0~1の範囲。0未満の場合、その行動は選択不可能。
            // 現在値に重みを乗算したものが最終的なスコアになる。
            _actions = new Dictionary<string, Score>()
            {
                { "MoveNorth", new Score(0.0f) },
                { "MoveSouth", new Score(0.0f) },
                { "MoveEast", new Score(0.0f) },
                { "MoveWest", new Score(0.0f) },
                { "MoveToEntrance", new Score(-1.0f) },
                { "MoveToArtifact", new Score(-1.0f) },
                { "AttackToEnemy", new Score(-1.0f) },
                { "AttackToAdventurer", new Score(-1.0f) },
                { "TalkWithAdventurer", new Score(-1.0f) },
                { "Scavenge", new Score(-1.0f) },
                { "RequestHelp", new Score(-1.0f) },
                { "ThrowItem", new Score(-1.0f) },
            };
        }

        public IReadOnlyList<string> GetEntries()
        {
            List<string> entries = new List<string>();
            foreach (KeyValuePair<string, Score> e in _actions)
            {
                if (0 <= e.Value.Current)
                {
                    entries.Add($"{e.Key} (Score: {e.Value.Total})");
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
            _debugView = entries;
#endif

            return entries;
        }

        public float GetScore(string action)
        {
            if (Check(action))
            {
                return _actions[action].Current;
            }
            else
            {
                return -1.0f;
            }
        }

        public void SetScore(string action, float score)
        {
            if (Check(action))
            {
                _actions[action].Current = Mathf.Clamp(score, -1.0f, 1.0f);
            }
        }

        public void ResetScore(string action)
        {
            if (Check(action))
            {
                _actions[action].Current = _actions[action].Default;
            }
        }

        public float GetWeight(string action)
        {
            if (Check(action))
            {
                return _actions[action].Weight;
            }
            else
            {
                return -1.0f;
            }
        }

        public void SetWeight(string action, float weight)
        {
            if (Check(action))
            {
                _actions[action].Weight = Mathf.Clamp01(weight);
            }
        }

        public void ResetWeight(string action)
        {
            if (Check(action))
            {
                _actions[action].Weight = 1.0f;
            }
        }

        bool Check(string action)
        {
            if (_actions.ContainsKey(action))
            {
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
