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

                // ���Ԋu�Ń����_���Ȗ`���҂��t�H�[�J�X����B
                int random = Random.Range(0, adventurerSpawner.Spawned.Count);
                Adventurer target = adventurerSpawner.Spawned[random];
                yield return FocusAsync(target);
            }
        }

        IEnumerator FocusAsync(Adventurer target)
        {
            const float Duration = 5.0f;
            const float RotateSpeed = 0.1f;

            // �`���҂��u�ړ��v�ȊO�̍s�������Ă���ꍇ�̓Y�[������B
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
                zoom = 2.0f; // �{���͓K���ɐݒ�B
            }

            // �ڕW���Y�[������B
            CinemachineFramingTransposer transposer = _vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
            float baseDistance = transposer.m_CameraDistance;
            transposer.m_CameraDistance = baseDistance / zoom;

            // ��莞�ԁA�J�������ڕW���t�H�[�J�X����B
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
