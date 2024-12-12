using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CameraFocus : MonoBehaviour
    {
        [SerializeField] CameraFocusUI[] _cameraFocusUI;

        bool[] _used;

        void Awake()
        {
            _used = new bool[_cameraFocusUI.Length];
        }

        public int RegisterTarget(GameObject target)
        {
            for (int i = 0; i < _used.Length; i++)
            {
                if (_used[i]) continue;

                _used[i] = true;
                _cameraFocusUI[i].SetTarget(target);

                return i;
            }

            Debug.LogWarning($"���g�p��{nameof(CameraFocusUI)}���Ȃ��B");

            return -1;
        }

        public void DeleteTarget(int id)
        {
            if (!IsWithinArray(id)) return;

            for (int i = 0; i < _used.Length; i++)
            {
                // ���ɖ��g�p��������ID���Ⴄ�ꍇ�B
                if (!_used[i] || i != id) continue;

                _used[i] = false;

                if (_cameraFocusUI[i] != null) _cameraFocusUI[i].SetTarget(null);

                return;
            }

            Debug.LogWarning($"���ɍ폜�ς݂�{nameof(CameraFocusUI)}�B: {id}");
        }

        bool IsWithinArray(int index)
        {
            if (0 <= index && index < _used.Length) return true;

            Debug.LogWarning($"ID�ɑΉ�����{nameof(CameraFocusUI)}�����݂��Ȃ��B: {index}");
            return false;
        }
    }
}
