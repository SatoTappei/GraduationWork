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
            // �ʒu���_���W�����̏o������ɂ��Ă����B
            follow.transform.position = new Vector3(11, 0, 8);
            // �J�������t�H�[�J�X���Ă���Ώۂ𒆐S�ɉ����E���Ăق����̂ŁA
            // AudioListener���A�^�b�`����Follow��ΏۂɒǏ]������B
            _audio = follow.AddComponent<AudioListener>();
            // AudioListener����������ƌx�����o��̂ŁA������Ԃł͖����ɂ��Ă����B
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
                zoom = 1.2f; // �{���͓K���ɐݒ�B
            }

            // �ڕW���Y�[������B
            float baseDistance = _transposer.m_CameraDistance;
            _transposer.m_CameraDistance = baseDistance / zoom;

            // ��莞�ԁA�J�������ڕW���t�H�[�J�X����B
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
