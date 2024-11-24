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
            _uiManager = UiManager.Find();
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
