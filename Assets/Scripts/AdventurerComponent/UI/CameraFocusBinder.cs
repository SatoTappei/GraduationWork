using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CameraFocusBinder : MonoBehaviour
    {
        Adventurer _adventurer;
        CameraManager _cameraManager;
        int _id;
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
                Debug.LogWarning("Šù‚É“o˜^Ï‚İB");
            }
            else
            {
                _isRegisterd = true;
                _id = _cameraManager.RegisterTarget(_adventurer);
            }
        }

        void OnDestroy()
        {
            if (_cameraManager != null)
            {
                _cameraManager.DeleteTarget(_id);
            }
        }
    }
}
