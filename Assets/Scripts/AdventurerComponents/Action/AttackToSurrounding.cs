using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class AttackToSurrounding : SurroundingAction
    {
        Adventurer _adventurer;
        Blackboard _blackboard;
        Animator _animator;
        ActionLog _actionLog;
        LineApply _line;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _blackboard = GetComponent<Blackboard>();
            _animator = GetComponentInChildren<Animator>();
            _actionLog = GetComponent<ActionLog>();
            _line = GetComponent<LineApply>();
        }

        public async UniTask AttackAsync<TTarget>(CancellationToken token) where TTarget : IDamageable
        {
            // �U���̌��ʂɂ���čs�����O�ɒǉ�������e���قȂ�B
            string actionLogText = string.Empty;

            // ���͂ɍU���\�ȑΏۂ����Ȃ��ꍇ�B
            if (!TryGetTarget<TTarget>(out Actor target))
            {
                _actionLog.Add("There are no enemies around to attack.");
                return;
            }

            // �U������O�ɖڕW�Ɍ����B
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(4.0f, targetPosition, token); // ��]���x�͓K���ɐݒ�B

            _animator.Play("Attack");
            _line.ShowLine(RequestLineType.Attack);

            // �_���[�W��^����B
            Character targetCharacter = target as Character;
            string result = string.Empty;
            if (targetCharacter == null)
            {
                result = "Miss";
            }
            else
            {
                // Defeat(���j����)�AHit(��������������)�ACorpse(���Ɏ���ł���)�AMiss(������Ȃ�����)
                result = targetCharacter.Damage(_blackboard.Attack, _adventurer.Coords);
            }

            // �U�����ʁB
            if (result == "Defeat")
            {
                _actionLog.Add("I attacked the enemy. I defeated the enemy.");
            }
            else if (result == "Hit")
            {
                _actionLog.Add("I attacked the enemy. The enemy is still alive.");
            }
            else if (result == "Corpse")
            {
                _actionLog.Add("I tried to attack it, but it was already dead.");
            }
            else if (result == "Miss")
            {
                // �܂��Ȃ�
            }
            else
            {
                Debug.LogWarning($"�U�����ʂɑΉ����鏈���������B�X�y���~�X�H: {result}");
            }

            // �U���̃A�j���[�V�����̍Đ��I����҂B
            await UniTask.WaitForSeconds(2.0f, cancellationToken: token);

            // �G�����j�����ꍇ�A�䎌�ƃ��O��\�����A���j�J�E���g�𑝂₷�B
            if (result == "Defeat")
            {
                _line.ShowLine(RequestLineType.DefeatEnemy);
                GameLog.Add($"�V�X�e��", $"{_blackboard.DisplayName}���G��|�����B", GameLogColor.White);

                _blackboard.DefeatCount++;
            }
        }
    }
}