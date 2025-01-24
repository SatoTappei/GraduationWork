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
            // �`���҂̔ԍ��ɑΉ�����UI�����邩�`�F�b�N�B
            int num = adventurer.AdventurerSheet.Number;
            if (num < 0 || _nameTagUI.Length <= num)
            {
                Debug.LogWarning($"�ԍ��ɑΉ�����UI�������B{num}");
                return;
            }

            // �`���҂̔ԍ��ɏd�����������`�F�b�N�B
            if (_used[num])
            {
                Debug.LogWarning($"�ԍ����d�����Ă���B{num}");
                return;
            }
            else
            {
                _used[num] = true;
            }

            for (int i = 0; i < _nameTagUI.Length; i++)
            {
                // �����ȊO�̖`���҂̖��O��\���������̂ŁA�����̔ԍ��ɑΉ�����UI�ȊO�ɑ΂��Ēǉ�����B
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
