using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ProfileWindow : MonoBehaviour
    {
        [SerializeField] ProfileWindowUI[] _profileWindowUI;

        bool[] _used;

        void Awake()
        {
            _used = new bool[_profileWindowUI.Length];
        }

        public int RegisterStatus(IProfileWindowDisplayStatus status)
        {
            for (int i = 0; i < _used.Length; i++)
            {
                if (_used[i]) continue;

                _used[i] = true;
                _profileWindowUI[i].SetStatus(status);

                return i;
            }

            Debug.LogWarning($"���g�p��{nameof(ProfileWindow)}���Ȃ��B");

            return -1;
        }

        public void UpdateStatus(int id, IProfileWindowDisplayStatus status)
        {
            if (IsInArray(id))
            {
                _profileWindowUI[id].UpdateStatus(status);
            }
        }

        public void DeleteStatus(int id)
        {
            if (!IsInArray(id)) return;

            for (int i = 0; i < _used.Length; i++)
            {
                if (_used[i] && i == id)
                {
                    _used[i] = false;
                    _profileWindowUI[i].DeleteStatus();

                    return;
                }
            }

            Debug.LogWarning($"���ɍ폜�ς݂�{nameof(ProfileWindow)}�B: {id}");
        }

        bool IsInArray(int index)
        {
            if (0 <= index && index < _used.Length) return true;

            Debug.LogWarning($"ID�ɑΉ�����{nameof(ProfileWindow)}�����݂��Ȃ��B: {index}");
            return false;
        }
    }
}