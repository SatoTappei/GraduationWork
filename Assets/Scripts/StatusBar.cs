using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatusBar : MonoBehaviour
    {
        [SerializeField] StatusBarUI[] _statusBarUI;

        bool[] _used;

        void Awake()
        {
            _used = new bool[_statusBarUI.Length];
        }

        public int RegisterStatus(IStatusBarDisplayStatus status)
        {
            for (int i = 0; i < _used.Length; i++)
            {
                if (_used[i]) continue;

                _used[i] = true;
                _statusBarUI[i].SetStatus(status);
                
                return i;
            }

            Debug.LogWarning($"���g�p�̃X�e�[�^�X�o�[���Ȃ��B");

            return -1;
        }

        public void UpdateStatus(int id, IStatusBarDisplayStatus status)
        {
            if (IsInArray(id))
            {
                _statusBarUI[id].UpdateStatus(status);
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
                    _statusBarUI[i].DeleteStatus();

                    return;
                }
            }

            Debug.LogWarning($"���ɍ폜�ς݂̃X�e�[�^�X�o�[�B: {id}");
        }

        public void ShowLine(int id, string line)
        {
            if (IsInArray(id))
            {
                _statusBarUI[id].ShowLine(line);
            }
        }

        bool IsInArray(int index)
        {
            if (0 <= index && index < _used.Length) return true;

            Debug.LogWarning($"ID�ɑΉ�����X�e�[�^�X�o�[�����݂��Ȃ��B: {index}");
            return false;
        }
    }
}
