using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CameraFocusBinder : MonoBehaviour
    {
        CameraFocus _cameraFocus;
        int _id;
        bool _isRegisterd;

        void Awake()
        {
            CameraFocus.TryFind(out _cameraFocus);
        }

        public void Register()
        {
            if (_isRegisterd)
            {
                Debug.LogWarning("ä˘Ç…ìoò^çœÇ›ÅB");
            }
            else
            {
                _isRegisterd = true;
                _id = _cameraFocus.RegisterTarget(gameObject);
            }
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
