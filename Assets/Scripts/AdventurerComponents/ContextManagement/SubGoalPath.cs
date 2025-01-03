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
                    _path.Add(gameObject.AddComponent<GetTreasure>());
                }
                else if (s == "依頼されたアイテムを手に入れる")
                {
                    _path.Add(gameObject.AddComponent<GetRequestedItem>());
                }
                else if (s == "ダンジョン内を探索する")
                {
                    _path.Add(gameObject.AddComponent<ExploreDungeon>());
                }
                else if (s == "自分より弱そうな敵を倒す")
                {
                    _path.Add(gameObject.AddComponent<DefeatWeakEnemy>());
                }
                else if (s == "強力な敵を倒す")
                {
                    _path.Add(gameObject.AddComponent<DefeatStrongEnemy>());
                }
                else if (s == "他の冒険者を倒す")
                {
                    _path.Add(gameObject.AddComponent<DefeatAdventurer>());
                }
                else if (s == "ダンジョンの入口に戻る")
                {
                    _path.Add(gameObject.AddComponent<ReturnToEntrance>());
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
                return false;
            }
            else if (_path[_currentIndex] == null)
            {
                Debug.LogWarning("サブゴールのクラスがnullになっている。");
                return false;
            }
            else
            {
                return _path[_currentIndex].IsCompleted();
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
