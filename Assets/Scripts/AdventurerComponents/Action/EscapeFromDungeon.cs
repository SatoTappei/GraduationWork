using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class EscapeFromDungeon : BaseAction
    {
        Blackboard _blackboard;
        SubGoalPath _subGoalPath;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _subGoalPath = GetComponent<SubGoalPath>();
        }

        public async UniTask<bool> EscapeAsync(CancellationToken token)
        {
            const float AnimationLength = 1.0f * 2;

            // ���݂̃T�u�S�[�����u�_���W�����̓����ɖ߂�B�v���A�T�u�S�[���������������`�F�b�N�B
            bool isLast = _subGoalPath.GetCurrent().Text.Japanese == ReturnToEntrance.JapaneseText;
            bool isCompleted = _subGoalPath.GetCurrent().IsCompleted();
            if (!(isLast && isCompleted)) return false;

            // �E�o�̉��o�B
            Animator animator = GetComponentInChildren<Animator>();
            animator.Play("Jump");

            // �E�o���̑䎌�B
            if (TryGetComponent(out LineApply line)) line.ShowLine(RequestLineType.Goal);

            // �Q�[���i�s���O�ɕ\���B
            GameLog.Add($"�V�X�e��", $"{_blackboard.DisplayName}���_���W��������E�o�����B", GameLogColor.Yellow);

            // ���o�̏I����҂B
            await UniTask.WaitForSeconds(AnimationLength, cancellationToken: token);

            // �Z������폜�B
            TryGetComponent(out Adventurer adventurer);
            DungeonManager.RemoveActorOnCell(adventurer.Coords, adventurer);

            return true;
        }
    }
}