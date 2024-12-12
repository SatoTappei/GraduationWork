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

            Debug.LogWarning($"未使用の{nameof(CameraFocusUI)}がない。");

            return -1;
        }

        public void DeleteTarget(int id)
        {
            if (!IsWithinArray(id)) return;

            for (int i = 0; i < _used.Length; i++)
            {
                // 既に未使用もしくはIDが違う場合。
                if (!_used[i] || i != id) continue;

                _used[i] = false;

                if (_cameraFocusUI[i] != null) _cameraFocusUI[i].SetTarget(null);

                return;
            }

            Debug.LogWarning($"既に削除済みの{nameof(CameraFocusUI)}。: {id}");
        }

        bool IsWithinArray(int index)
        {
            if (0 <= index && index < _used.Length) return true;

            Debug.LogWarning($"IDに対応する{nameof(CameraFocusUI)}が存在しない。: {index}");
            return false;
        }
    }
}
