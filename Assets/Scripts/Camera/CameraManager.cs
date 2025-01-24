using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] TargetFocusCamera[] _cameras;

        bool[] _used;

        void Awake()
        {
            _used = new bool[_cameras.Length];
        }

        public static CameraManager Find()
        {
            return GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
        }

        public int RegisterTarget(Adventurer target)
        {
            for (int i = 0; i < _used.Length; i++)
            {
                if (_used[i]) continue;

                _used[i] = true;
                _cameras[i].SetTarget(target);

                return i;
            }

            Debug.LogWarning($"���g�p��{nameof(TargetFocusCamera)}���Ȃ��B");

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

                if (_cameras[i] != null) _cameras[i].DeleteTarget();

                return;
            }

            Debug.LogWarning($"���ɍ폜�ς݂�{nameof(TargetFocusCamera)}�B: {id}");
        }

        bool IsWithinArray(int index)
        {
            if (0 <= index && index < _used.Length) return true;

            Debug.LogWarning($"ID�ɑΉ�����{nameof(TargetFocusCamera)}�����݂��Ȃ��B: {index}");
            return false;
        }
    }
}
