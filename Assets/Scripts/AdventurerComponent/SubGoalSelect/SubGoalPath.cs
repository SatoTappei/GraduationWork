using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class SubGoalPath : MonoBehaviour
    {
        List<SubGoal> _path;
        int _currentIndex;

        public IReadOnlyList<SubGoal> Path => _path;

        void Awake()
        {
            _path = new List<SubGoal>();
        }

        public void Initialize(IReadOnlyList<string> subGoals)
        {
            // サブゴールに対応したコンポーネントを追加。
            foreach (string s in subGoals)
            {
                if (s == nameof(FindTreasureGoal))
                {
                    _path.Add(gameObject.AddComponent<FindTreasureGoal>());
                }
                else if (s == nameof(FindItemGoal))
                {
                    _path.Add(gameObject.AddComponent<FindItemGoal>());
                }
                else if (s == nameof(DefeatEnemyGoal))
                {
                    _path.Add(gameObject.AddComponent<DefeatEnemyGoal>());
                }
                else if (s == nameof(DefeatBossGoal))
                {
                    _path.Add(gameObject.AddComponent<DefeatBossGoal>());
                }
                else if (s == nameof(DefeatAdventurerGoal))
                {
                    _path.Add(gameObject.AddComponent<DefeatAdventurerGoal>());
                }
                else if (s == nameof(ReturnEntranceGoal))
                {
                    _path.Add(gameObject.AddComponent<ReturnEntranceGoal>());
                }
                else
                {
                    Debug.LogWarning($"対応するサブゴールのクラスが無い。スペルミス？: {s}");
                }
            }
        }

        public SubGoal GetCurrent()
        {
            if (_path.Count == 0)
            {
                Debug.LogWarning("サブゴールが設定されておらず、現在のサブゴールを取得できない。");
                return null;
            }
            else
            {
                return _path[_currentIndex];
            }
        }

        public bool IsAchieve()
        {
            if (_path.Count == 0)
            {
                Debug.LogWarning("サブゴールが設定されておらず、達成したか判定できない。");
            }
            else if (_path[_currentIndex] == null)
            {
                Debug.LogWarning("サブゴールのクラスがnullになっている。");
            }

            if (_path[_currentIndex].Check() == SubGoal.State.Completed)
            {
                return true;
            }
            else if (_path[_currentIndex].Check() == SubGoal.State.Failed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetNext()
        {
            if (_path.Count == 0)
            {
                Debug.LogWarning("サブゴールが設定されておらず、次のサブゴールに進めない。");
            }
            else
            {
                _currentIndex++;
                _currentIndex = Mathf.Min(_currentIndex, _path.Count - 1);
            }
        }
    }
}
