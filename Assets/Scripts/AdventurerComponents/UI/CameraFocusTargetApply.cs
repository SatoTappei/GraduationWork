using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CameraFocusTargetApply : MonoBehaviour
    {
        CameraFocus _cameraFocus;
        int _id;

        void Awake()
        {
            CameraFocus.TryFind(out _cameraFocus);
        }

        public void Register()
        {
            _id = _cameraFocus.RegisterTarget(gameObject);
        }

        void OnDestroy()
        {
            if (_cameraFocus != null)
            {
                _cameraFocus.DeleteTarget(_id);
            }
        }
    }
}
