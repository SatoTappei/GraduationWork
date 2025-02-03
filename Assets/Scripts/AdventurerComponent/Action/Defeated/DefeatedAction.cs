using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VTNConnect;

namespace Game
{
    public class DefeatedAction : BaseAction
    {
        [SerializeField] AudioClip _defeatedSE;

        Adventurer _adventurer;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
        }

        public async UniTask<bool> PlayAsync(CancellationToken token)
        {
            const float PlayTime = 2.5f;

            // �������Ă���ꍇ�B
            if (_adventurer.Status.IsAlive) return false;

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
            GameLog.Add(
                "�V�X�e��", 
                $"�͐s�����c", 
                LogColor.Red,
                _adventurer.Sheet.DisplayID
            );

            // �C�x���g�𑗐M�B
            EventData eventData1 = new EventData(EventDefine.DeathScream);
            VantanConnect.SendEvent(eventData1);
            EventData eventData2 = new EventData(EventDefine.KnockWindow);
            VantanConnect.SendEvent(eventData2);

            // ���o�̏I����҂B
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            return true;
        }
    }
}
