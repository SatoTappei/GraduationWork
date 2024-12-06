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
                // ���Ԋu�Ń����_���Ȗ`���҂��t�H�[�J�X����B
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

            // �`���҂��u�ړ��v�ȊO�̍s�������Ă���ꍇ�̓Y�[������B
            float zoom = 1.0f;
            if (target.SelectedAction == "Attack Surrounding" ||
                target.SelectedAction == "Scavenge Surrounding" ||
                target.SelectedAction == "Talk Surrounding")
            {
                zoom = 2.0f; // �{���͓K���ɐݒ�B
            }

            // �ڕW���Y�[������B
            CinemachineFramingTransposer transposer = _vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
            float baseDistance = transposer.m_CameraDistance;
            transposer.m_CameraDistance = baseDistance / zoom;

            // ��莞�ԁA�J�������ڕW���t�H�[�J�X����B
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
