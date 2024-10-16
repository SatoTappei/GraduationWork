using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] BadgeUI[] _badges;

        bool[] _used;

        void Awake()
        {
            _used = new bool[_badges.Length];
        }

        void Start()
        {

        }

        public static UiManager Find()
        {
            return FindAnyObjectByType<UiManager>();
        }

        public int AddBadge(Adventure adventure)
        {
            for (int i = 0; i < _used.Length; i++)
            {
                if (_used[i]) continue;

                _used[i] = true;
                _badges[i].SetAdventureData();

                return i;
            }

            Debug.LogWarning($"����ȏ�o�b�W��ǉ��o���Ȃ��B: {adventure.name}");

            return -1;
        }

        public void UpdateBadge(int id, Adventure adventure)
        {
            _badges[id].UpdateAdventureData();
        }

        public void ShowAdventureLine(int id, string line)
        {

        }

        public void RemoveBadge(int id)
        {
            if (id < 0 || _used.Length <= id)
            {
                Debug.LogWarning($"ID�ɑΉ�����o�b�W�����݂��Ȃ��B: {id}");
                return;
            }

            for (int i = 0; i < _used.Length; i++)
            {
                if (_used[i] && i == id)
                {
                    _used[i] = false;
                    _badges[i].DeleteAdventureData();

                    return;
                }
            }

            Debug.LogWarning($"���ɍ폜�ς݂̃o�b�W�B: {id}");
        }
    }
}

// �A�C�R���A�\�����A�̗́A�S��
// ���O�A�L�����N�^�[�ݒ�
// �̗͂ƐS��̍X�V
// �Z���t�̕\���B