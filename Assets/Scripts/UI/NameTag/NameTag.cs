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

        public void Register(Adventurer adventurer)
        {
            // 冒険者の番号に対応するUIがあるかチェック。
            int num = adventurer.AdventurerSheet.Number;
            if (num < 0 || _nameTagUI.Length <= num)
            {
                Debug.LogWarning($"番号に対応するUIが無い。{num}");
                return;
            }

            // 冒険者の番号に重複が無いかチェック。
            if (_used[num])
            {
                Debug.LogWarning($"番号が重複している。{num}");
                return;
            }
            else
            {
                _used[num] = true;
            }

            for (int i = 0; i < _nameTagUI.Length; i++)
            {
                // 自分以外の冒険者の名前を表示したいので、自分の番号に対応するUI以外に対して追加する。
                if (i != num)
                {
                    _nameTagUI[i].Add(adventurer);
                }
            }
        }

        public void Delete(Adventurer adventurer)
        {
            foreach (NameTagUI ui in _nameTagUI)
            {
                ui.Remove(adventurer);
            }
        }
    }
}
