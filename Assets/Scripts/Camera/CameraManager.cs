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

        public void RegisterTarget(int displayID, Adventurer target)
        {
            if (!IsInArray(displayID)) return;

            if (_used[displayID])
            {
                Debug.LogWarning($"���ɓo�^�ς݁B{displayID}");
                return;
            }

            _used[displayID] = true;
            _cameras[displayID].SetTarget(target);
        }

        public void DeleteTarget(int displayID)
        {
            if (!IsInArray(displayID)) return;

            if (!_used[displayID])
            {
                Debug.LogWarning($"���ɍ폜�ς݁B{displayID}");
                return;
            }

            _used[displayID] = false;

            if (_cameras[displayID] != null)
            {
                _cameras[displayID].DeleteTarget();
            }
        }

        public void Shake()
        {
            foreach (TargetFocusCamera c in _cameras)
            {
                c.Shake();
            }
        }

        bool IsInArray(int index)
        {
            if (0 <= index && index < _used.Length) return true;

            Debug.LogWarning($"�Ή�����{nameof(TargetFocusCamera)}�����݂��Ȃ��B: {index}");
            return false;
        }
    }
}
