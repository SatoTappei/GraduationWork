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
                if (s == "お宝を手に入れる")
                {
                    _path.Add(gameObject.AddComponent<FindTreasureGoal>());
                }
                else if (s == "依頼されたアイテムを手に入れる")
                {
                    _path.Add(gameObject.AddComponent<FindItemGoal>());
                }
                else if (s == "ダンジョン内を探索する")
                {
                    _path.Add(gameObject.AddComponent<ExploreDungeonGoal>());
                }
                else if (s == "自分より弱そうな敵を倒す")
                {
                    _path.Add(gameObject.AddComponent<DefeatEnemyGoal>());
                }
                else if (s == "強力な敵を倒す")
                {
                    _path.Add(gameObject.AddComponent<DefeatBossGoal>());
                }
                else if (s == "他の冒険者を倒す")
                {
                    _path.Add(gameObject.AddComponent<DefeatAdventurerGoal>());
                }
                else if (s == "ダンジョンの入口に戻る")
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

        public bool IsAchieve(out string result)
        {
            if (_path.Count == 0)
            {
                Debug.LogWarning("サブゴールが設定されておらず、達成したか判定できない。");
                result = string.Empty;
            }
            else if (_path[_currentIndex] == null)
            {
                Debug.LogWarning("サブゴールのクラスがnullになっている。");
                result = string.Empty;
            }
            else if (_path[_currentIndex].IsCompleted())
            {
                result = "Completed";
            }
            else if (_path[_currentIndex].IsRetire())
            {
                result = "Retire";
            }
            else
            {
                result = string.Empty;
            }

            return result != string.Empty;
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
