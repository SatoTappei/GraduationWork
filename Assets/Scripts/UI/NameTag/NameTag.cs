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
                Debug.LogWarning($"���ɓo�^�ς݁B{displayID}");
                return;
            }

            _used[displayID] = true;

            for (int i = 0; i < _nameTagUI.Length; i++)
            {
                // �����ȊO�̖`���҂̖��O��\���������̂ŁA�����̔ԍ��ɑΉ�����UI�ȊO�ɑ΂��Ēǉ�����B
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
                Debug.LogWarning($"���ɍ폜�ς݁B{displayID}");
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

            Debug.LogWarning($"�Ή�����{nameof(ProfileWindow)}�����݂��Ȃ��B: {index}");
            return false;
        }
    }
}
