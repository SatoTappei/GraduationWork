using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CameraFocusTargetApply : MonoBehaviour
    {
        UiManager _uiManager;
        int _id;

        void Awake()
        {
            UiManager.TryFind(out _uiManager);
        }

        public void Register()
        {
            _id = _uiManager.RegisterCameraFocusTarget(gameObject);
        }

        void OnDestroy()
        {
            if (_uiManager != null)
            {
                _uiManager.DeleteCameraFocusTarget(_id);
            }
        }
    }
}
