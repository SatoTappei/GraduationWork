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
                    // ���Ԋu�Ń����_���Ȗ`���҂��t�H�[�J�X����B
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
            float baseDistance = _transposer.m_CameraDistance;
            _transposer.m_CameraDistance = baseDistance / zoom;

            // ��莞�ԁA�J�������ڕW���t�H�[�J�X����B
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
