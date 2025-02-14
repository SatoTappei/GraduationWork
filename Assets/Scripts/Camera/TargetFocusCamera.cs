using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class TargetFocusCamera : MonoBehaviour
    {
        CinemachineVirtualCamera _vcam;
        CinemachineFramingTransposer _transposer;
        CinemachineImpulseSource _impulse;
        Coroutine _focus;

        void Awake()
        {
            _vcam = GetComponent<CinemachineVirtualCamera>();
            _transposer = _vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
            _impulse = GetComponent<CinemachineImpulseSource>();

            GameObject follow = new GameObject($"{name}_Follow");
            _vcam.Follow = follow.transform;
            // 位置をダンジョンの出入り口にしておく。
            follow.transform.position = new Vector3(11, 0, 8);
        }

        public void SetTarget(Adventurer target)
        {
            _focus = StartCoroutine(FocusRepeatingAsync(target));
        }

        public void DeleteTarget()
        {
            if (_focus != null) StopCoroutine(_focus);
        }

        public void Shake()
        {
            _impulse.GenerateImpulse();
        }

        IEnumerator FocusRepeatingAsync(Adventurer target)
        {
            while (true)
            {
                yield return FocusAsync(target);
            }
        }

        IEnumerator FocusAsync(Adventurer target)
        {
            const float Duration = 5.0f;

            // 冒険者が「移動」以外の行動をしている場合はズームする。
            float zoom;
            if (target.SelectedAction == "MoveNorth" ||
                target.SelectedAction == "MoveSouth" ||
                target.SelectedAction == "MoveEast" ||
                target.SelectedAction == "MoveWest")
            {
                zoom = 1.0f;
            }
            else
            {
                zoom = 1.2f; // 倍率は適当に設定。
            }

            // 目標をズームする。
            float baseDistance = _transposer.m_CameraDistance;
            _transposer.m_CameraDistance = baseDistance / zoom;

            // 一定時間、カメラが目標をフォーカスする。
            for (float f = 0; f < Duration; f += Time.deltaTime)
            {
                if (target == null) break;

                _vcam.Follow.transform.position = target.transform.position;
                //_vcam.transform.RotateAround(_vcam.transform.position, Vector3.up, RotateSpeed);

                yield return null;
            }

            _transposer.m_CameraDistance = baseDistance;
        }
    }
}
