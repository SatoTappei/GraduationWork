using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EntryAction : BaseAction
    {
        [SerializeField] AudioClip _entrySE;

        public void Play()
        {
            // �o�ꎞ�̉��o�B
            TryGetComponent(out AudioSource audioSource);
            if (_entrySE != null)
            {
                audioSource.clip = _entrySE;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("���ꎞ��SE���A�T�C������Ă��Ȃ��B");
            }

            // �o�ꎞ�̑䎌�B
            TryGetComponent(out LineDisplayer line);
            line.Show(RequestLineType.Entry);

            // �Q�[���i�s���O�ɕ\���B
            TryGetComponent(out Adventurer adventurer);
            GameLog.Add(
                $"�V�X�e��", 
                $"�_���W�����ɂ���Ă����B", 
                LogColor.White,
                adventurer.Sheet.DisplayID
            );
        }
    }
}
