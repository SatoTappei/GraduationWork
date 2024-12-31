using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum CameraFocusPriority
    {
        Critical,
        High,
        Normal,
        Low,
    }

    public class TargetFocusCamera : MonoBehaviour
    {
        CinemachineVirtualCamera _vcam;

        void Awake()
        {
            _vcam = GetComponent<CinemachineVirtualCamera>();
        }

        void Start()
        {
            StartCoroutine(UpdateAsync());
        }

        public static bool TryFind(out TargetFocusCamera result)
        {
            result = GameObject.FindGameObjectWithTag("BirdsEyeViewCamera").GetComponent<TargetFocusCamera>();
            return result != null;
        }

        IEnumerator UpdateAsync()
        {
            AdventurerSpawner.TryFind(out AdventurerSpawner adventurerSpawner);
            while (true)
            {
                if (adventurerSpawner.Spawned == null) yield return null;
                else if (adventurerSpawner.Spawned.Count == 0) yield return null;

                // 一定間隔でランダムな冒険者をフォーカスする。
                int random = Random.Range(0, adventurerSpawner.Spawned.Count);
                Adventurer target = adventurerSpawner.Spawned[random];
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
                zoom = 2.0f; // 倍率は適当に設定。
            }

            // 目標をズームする。
            CinemachineFramingTransposer transposer = _vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
            float baseDistance = transposer.m_CameraDistance;
            transposer.m_CameraDistance = baseDistance / zoom;

            // 一定時間、カメラが目標をフォーカスする。
            for (float f = 0; f < Duration; f += Time.deltaTime)
            {
                if (target == null) break;

                _vcam.Follow.position = target.transform.position;
                _vcam.transform.RotateAround(_vcam.transform.position, Vector3.up, RotateSpeed);

                yield return null;
            }

            transposer.m_CameraDistance = baseDistance;
        }
    }
}
