using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class 詰み防止 : MonoBehaviour
    {
        List<string> _history;
        int _checkIndex;

        void Awake()
        {
            _history = new List<string>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(!Check("Attack Surrounding"))
                {
                    Debug.Log("正常");
                }
            }
        }

        public bool Check(string action)
        {
            // 10回以上行動を繰り返す、2箇所の往復(行動2回)、3箇所の往復(行動4回)を検知できるのが20。
            if (_history.Count >= 20) _history.RemoveAt(0);

            _history.Add(action);

            // チェック項目が増えた場合の処理負荷の増加を防ぐため、毎ターン1項目ずつチェックする。
            if (_checkIndex == 0)
            {
                if (IsActionRepeated()) { LogWarning("同じ動作を10回以上繰り返している。"); return true; }
            }
            else if (_checkIndex == 1)
            {
                if (Is2LoopMoving()) { LogWarning("2箇所を5往復以上している。"); return true; }
            }
            else if (_checkIndex == 2)
            {
                if (Is3LoopMoving()) { LogWarning("3箇所を5往復以上している。"); return true; }
            }

            _checkIndex++;
            _checkIndex %= 3;

            return false;
        }

        bool IsActionRepeated()
        {
            if (_history.Count < 10) return false;

            string action = _history[0];

            // 移動は10回以上繰り返されても大丈夫。
            if (IsMoveAction(action)) return false;

            for (int i = 1; i < 10; i++)
            {
                if (action != _history[i]) return false;
            }

            return true;
        }

        bool Is2LoopMoving()
        {
            if (_history.Count < 10) return false;

            string action1 = _history[0];
            string action2 = _history[1];

            if (IsMoveAction(action1) && IsMoveAction(action2))
            {
                for (int i = 2; i < 10; i += 2)
                {
                    if (action1 != _history[i]) return false;
                    if (action2 != _history[i + 1]) return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        bool Is3LoopMoving()
        {
            if (_history.Count < 20) return false;

            string action1 = _history[0];
            string action2 = _history[1];
            string action3 = _history[2];
            string action4 = _history[3];

            if (IsMoveAction(action1) && IsMoveAction(action2) && IsMoveAction(action3) && IsMoveAction(action4))
            {
                for (int i = 4; i < 20; i += 4)
                {
                    if (action1 != _history[i]) return false;
                    if (action2 != _history[i + 1]) return false;
                    if (action3 != _history[i + 2]) return false;
                    if (action4 != _history[i + 3]) return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        static bool IsMoveAction(string action)
        {
            return action == "Move North" || 
                   action == "Move South" || 
                   action == "Move East" || 
                   action == "Move West";
        }

        void LogWarning(string text)
        {
            if (TryGetComponent(out Adventurer adventurer))
            {
                Debug.LogWarning($"{adventurer.AdventurerSheet.FullName}: {text}");
            }
            else
            {
                Debug.LogWarning(text);
            }
        }
    }
}
