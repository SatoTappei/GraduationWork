using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CameraFocusUI : MonoBehaviour
    {
        [SerializeField] GameObject _target;

        public void SetTarget(GameObject target)
        {
            _target = target;
        }

        public void Execute()
        {
            if (_target == null) return;

            if (FreeMovableCamera.TryFind(out FreeMovableCamera camera))
            {
                camera.SetPosition(_target.transform.position);
            }

            if (TryGetComponent(out AudioSource audio))
            {
                audio.Play();
            }
        }
    }
}
