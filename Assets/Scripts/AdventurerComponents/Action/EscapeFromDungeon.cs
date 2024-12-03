using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class EscapeFromDungeon : BaseAction
    {
        Adventurer _adventurer;
        Blackboard _blackboard;
        SubGoalPath _subGoalPath;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _blackboard = GetComponent<Blackboard>();
            _subGoalPath = GetComponent<SubGoalPath>();
        }

        public async UniTask<bool> EscapeAsync(CancellationToken token)
        {
            const float AnimationLength = 1.0f * 2;

            // �Ō�̃T�u�S�[�����N���A������Ԃ������ɗ����Ă���ꍇ�͒E�o�B
            bool isLast = _subGoalPath.IsLast;
            bool isEntrance = Blueprint.Interaction[_adventurer.Coords.y][_adventurer.Coords.x] == '<';
            bool isCompleted = _subGoalPath.Current.IsCompleted();

            if (!(isLast && isEntrance && isCompleted)) return false;

            // �E�o�̉��o�B
            Animator animator = GetComponentInChildren<Animator>();
            animator.Play("Jump");

            // �E�o���̑䎌�B
            if (TryGetComponent(out LineApply line)) line.ShowLine(RequestLineType.Goal);

            // �Q�[���i�s���O�ɕ\���B
            UiManager.TryFind(out UiManager uiManager);
            uiManager.AddLog($"{_blackboard.DisplayName}���_���W��������E�o�����B");

            // ���o�̏I����҂B
            await UniTask.WaitForSeconds(AnimationLength, cancellationToken: token);

            // �Z������폜�B
            TryGetComponent(out Adventurer adventurer);
            DungeonManager.TryFind(out DungeonManager dungeonManager);
            dungeonManager.RemoveActorOnCell(adventurer.Coords, adventurer);

            return true;
        }
    }
}