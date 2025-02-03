using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class NameTag : MonoBehaviour
    {
        [SerializeField] NameTagUI[] _nameTagUI;

        bool[] _used;

        void Awake()
        {
            _used = new bool[_nameTagUI.Length];
        }

        public static NameTag Find()
        {
            return GameObject.FindGameObjectWithTag("UiManager").GetComponent<NameTag>();
        }

        public void Register(int displayID, Adventurer adventurer)
        {
            if (!IsInArray(displayID)) return;

            if (_used[displayID])
            {
                Debug.LogWarning($"既に登録済み。{displayID}");
                return;
            }

            _used[displayID] = true;

            for (int i = 0; i < _nameTagUI.Length; i++)
            {
                // 自分以外の冒険者の名前を表示したいので、自分の番号に対応するUI以外に対して追加する。
                if (i != displayID)
                {
                    _nameTagUI[i].Add(adventurer);
                }
            }
        }

        public void Delete(int displayID, Adventurer adventurer)
        {
            if (!IsInArray(displayID)) return;

            if (!_used[displayID])
            {
                Debug.LogWarning($"既に削除済み。{displayID}");
                return;
            }

            _used[displayID] = false;

            for (int i = 0; i < _nameTagUI.Length; i++)
            {
                if (i != displayID)
                {
                    _nameTagUI[i].Remove(adventurer);
                }
            }
        }

        bool IsInArray(int index)
        {
            if (0 <= index && index < _used.Length) return true;

            Debug.LogWarning($"対応する{nameof(ProfileWindow)}が存在しない。: {index}");
            return false;
        }
    }
}
