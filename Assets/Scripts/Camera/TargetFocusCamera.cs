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

        AudioListener _audio;
        Coroutine _focus;

        void Awake()
        {
            _vcam = GetComponent<CinemachineVirtualCamera>();
            _transposer = _vcam.GetCinemachineComponent<CinemachineFramingTransposer>();

            GameObject follow = new GameObject($"{name}_Follow");
            _vcam.Follow = follow.transform;
            // 位置をダンジョンの出入り口にしておく。
            follow.transform.position = new Vector3(11, 0, 8);
            // カメラがフォーカスしている対象を中心に音を拾ってほしいので、
            // AudioListenerをアタッチしたFollowを対象に追従させる。
            _audio = follow.AddComponent<AudioListener>();
            // AudioListenerが複数あると警告が出るので、初期状態では無効にしておく。
            _audio.enabled = false;
        }

        public void SetTarget(Adventurer target)
        {
            _focus = StartCoroutine(FocusRepeatingAsync(target));
        }

        public void DeleteTarget()
        {
            if (_focus != null) StopCoroutine(_focus);
        }

        public void EnableAudio(bool value)
        {
            _audio.enabled = value;
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
            const float RotateSpeed = 0.1f;

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
                _vcam.transform.RotateAround(_vcam.transform.position, Vector3.up, RotateSpeed);

                yield return null;
            }

            _transposer.m_CameraDistance = baseDistance;
        }
    }
}
