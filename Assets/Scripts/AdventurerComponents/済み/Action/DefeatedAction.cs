using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class DefeatedAction : BaseAction
    {
        [SerializeField] AudioClip _defeatedSE;

        Blackboard _blackboard;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
        }

        public async UniTask<bool> PlayAsync(CancellationToken token)
        {
            const float PlayTime = 2.5f;

            // �������Ă���ꍇ�B
            if (_blackboard.IsAlive) return false;

            // ���S���̃A�j���[�V�����Đ��B
            Animator animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.Play("Death");
            }

            // ���S����SE�Đ��B
            AudioSource audioSource = GetComponent<AudioSource>();
            if (_defeatedSE != null)
            {
                audioSource.clip = _defeatedSE;
                audioSource.Play();
            }
            else 
            {
                Debug.LogWarning("�͐s�����ۂ�SE���A�T�C������Ă��Ȃ��B");
            }

            // ���S���̑䎌�B
            if (TryGetComponent(out LineDisplayer line))
            {
                line.ShowLine(RequestLineType.Defeated);
            }

            // ���O�ɕ\���B
            Adventurer adventurer = GetComponent<Adventurer>();
            GameLog.Add(
                "�V�X�e��", 
                $"{adventurer.AdventurerSheet.DisplayName}�͗͐s�����B", 
                GameLogColor.Red
            );

            // ���o�̏I����҂B
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            return true;
        }
    }
}
