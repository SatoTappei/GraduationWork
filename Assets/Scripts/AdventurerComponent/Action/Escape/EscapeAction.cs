using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class EscapeAction : BaseAction
    {
        SubGoalPath _subGoalPath;

        void Awake()
        {
            _subGoalPath = GetComponent<SubGoalPath>();
        }

        public async UniTask<bool> PlayAsync(CancellationToken token)
        {
            const float PlayTime = 1.0f * 2;

            // �T�u�R�[�����ݒ肳��Ă��Ȃ��ꍇ�B
            if (_subGoalPath.GetCurrent() == null) return false;

            // ���݂̃T�u�S�[�����u�_���W�����̓����ɖ߂�B�v���A�T�u�S�[���������������`�F�b�N�B
            bool isLast = _subGoalPath.GetCurrent().Description.Japanese == "�_���W�����̓����ɖ߂�B";
            bool isCompleted = _subGoalPath.GetCurrent().Check() == SubGoal.State.Completed;
            if (!(isLast && isCompleted)) return false;

            // �E�o�̉��o�B
            Animator animator = GetComponentInChildren<Animator>();
            animator.Play("Jump");

            // �E�o���̑䎌�B
            TryGetComponent(out LineDisplayer line);
            line.ShowLine(RequestLineType.Goal);

            // �Q�[���i�s���O�ɕ\���B
            TryGetComponent(out Adventurer adventurer);
            GameLog.Add(
                $"�V�X�e��", 
                $"{adventurer.AdventurerSheet.DisplayName}���_���W��������E�o�����B", 
                GameLogColor.Yellow
            );

            // ���o�̏I����҂B
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            return true;
        }
    }
}