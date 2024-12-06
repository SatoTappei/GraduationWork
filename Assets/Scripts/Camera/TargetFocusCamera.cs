using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                // 一定間隔でランダムな冒険者をフォーカスする。
                Adventurer[] spawned = adventurerSpawner.Spawned.Where(a => a != null).ToArray();
                if (spawned.Length > 0)
                {
                    yield return FocusRandomAdventurerAsync(spawned);
                }

                yield return null;
            }
        }

        IEnumerator FocusRandomAdventurerAsync(Adventurer[] adventurers)
        {
            Adventurer target = adventurers[Random.Range(0, adventurers.Length)];

            // 冒険者が「移動」以外の行動をしている場合はズームする。
            float zoom = 1.0f;
            if (target.SelectedAction == "Attack Surrounding" ||
                target.SelectedAction == "Scavenge Surrounding" ||
                target.SelectedAction == "Talk Surrounding")
            {
                zoom = 2.0f; // 倍率は適当に設定。
            }

            // 目標をズームする。
            CinemachineFramingTransposer transposer = _vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
            float baseDistance = transposer.m_CameraDistance;
            transposer.m_CameraDistance = baseDistance / zoom;

            // 一定時間、カメラが目標をフォーカスする。
            yield return FocusAsync(target.transform);

            transposer.m_CameraDistance = baseDistance;
        }

        IEnumerator FocusAsync(Transform target)
        {
            const float Duration = 5.0f;
            const float RotateSpeed = 0.1f;

            for (float f = 0; f < Duration; f += Time.deltaTime)
            {
                if (target == null) break;

                _vcam.Follow.position = target.transform.position;
                _vcam.transform.RotateAround(_vcam.transform.position, Vector3.up, RotateSpeed);

                yield return null;
            }
        }
    }
}
