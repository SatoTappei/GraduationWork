using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CameraFocusUI : MonoBehaviour
    {
        [SerializeField] GameObject _target;

        AudioSource _audioSource;
        FreeMovableCamera _camera;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _camera = FreeMovableCamera.Find();
        }

        public void SetTarget(GameObject target)
        {
            _target = target;
        }

        public void Focus()
        {
            if (_target == null)
            {
                Debug.LogWarning("カメラをフォーカスする対象がアタッチされていない。");
            }
            else
            {
                _audioSource.Play();
                _camera.SetPosition(_target.transform.position);
            }
        }
    }
}
