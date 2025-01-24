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

            Debug.LogWarning($"未使用の{nameof(TargetFocusCamera)}がない。");

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

                if (_cameras[i] != null) _cameras[i].DeleteTarget();

                return;
            }

            Debug.LogWarning($"既に削除済みの{nameof(TargetFocusCamera)}。: {id}");
        }

        bool IsWithinArray(int index)
        {
            if (0 <= index && index < _used.Length) return true;

            Debug.LogWarning($"IDに対応する{nameof(TargetFocusCamera)}が存在しない。: {index}");
            return false;
        }
    }
}
