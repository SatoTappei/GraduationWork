using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CameraFocusBinder : MonoBehaviour
    {
        Adventurer _adventurer;
        CameraManager _cameraManager;
        bool _isRegisterd;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _cameraManager = CameraManager.Find();
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
                _cameraManager.RegisterTarget(_adventurer.Sheet.DisplayID, _adventurer);
            }
        }

        void OnDestroy()
        {
            if (_cameraManager != null)
            {
                _cameraManager.DeleteTarget(_adventurer.Sheet.DisplayID);
            }
        }
    }
}
