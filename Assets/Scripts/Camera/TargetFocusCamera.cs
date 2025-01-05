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
        AdventurerSpawner _spawner;

        void Awake()
        {
            _vcam = GetComponent<CinemachineVirtualCamera>();
            _transposer = _vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
            _spawner = AdventurerSpawner.Find();
        }

        void Start()
        {
            StartCoroutine(UpdateAsync());
        }

        public static TargetFocusCamera Find()
        {
            return GameObject.FindGameObjectWithTag("BirdsEyeViewCamera").GetComponent<TargetFocusCamera>();
        }

        IEnumerator UpdateAsync()
        {
            while (true)
            {
                if (0 < _spawner.Spawned.Count)
                {
                    // 一定間隔でランダムな冒険者をフォーカスする。
                    int random = Random.Range(0, _spawner.Spawned.Count);
                    Adventurer target = _spawner.Spawned[random];
                    yield return FocusAsync(target);
                }
                else
                {
                    yield return null;
                }
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
                zoom = 2.0f; // 倍率は適当に設定。
            }

            // 目標をズームする。
            float baseDistance = _transposer.m_CameraDistance;
            _transposer.m_CameraDistance = baseDistance / zoom;

            // 一定時間、カメラが目標をフォーカスする。
            for (float f = 0; f < Duration; f += Time.deltaTime)
            {
                if (target == null) break;

                _vcam.Follow.position = target.transform.position;
                _vcam.transform.RotateAround(_vcam.transform.position, Vector3.up, RotateSpeed);

                yield return null;
            }

            _transposer.m_CameraDistance = baseDistance;
        }
    }
}
