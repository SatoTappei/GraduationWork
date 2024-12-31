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

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _blackboard = GetComponent<Blackboard>();
            _animator = GetComponentInChildren<Animator>();
        }

        public async UniTask AttackAsync<TTarget>(CancellationToken token) where TTarget : IDamageable
        {
            // �V���A���C�Y���Ă��ǂ��B
            const float RotateSpeed = 4.0f;
            const string Weapon = "�p���`";

            // �U���̌��ʂɂ���čs�����O�ɒǉ�������e���قȂ�B
            string actionLogText = string.Empty;

            // ���͂ɍU���\�ȑΏۂ�����ꍇ�͍U���B
            if (TryGetTarget<TTarget>(out Actor target))
            {
                // �U������O�ɖڕW�Ɍ����B
                Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
                await RotateAsync(RotateSpeed, targetPosition, token);

                _animator.Play("Attack");

                // �U�����̑䎌�B
                if (TryGetComponent(out LineApply line)) line.ShowLine(RequestLineType.Attack);

                // �_���[�W��^����B
                string attackResult = ApplyDamage(target as Character, Weapon, _blackboard.Attack);
                if (attackResult == "Defeat")
                {
                    // ���j���̑䎌�B
                    if (line != null) line.ShowLine(RequestLineType.DefeatEnemy);

                    // �Q�[���i�s���O�ɕ\���B
                    GameLog.Add($"�V�X�e��", $"{_blackboard.DisplayName}���G��|�����B", GameLogColor.White);

                    actionLogText = "I attacked the enemy. I defeated the enemy.";

                    _blackboard.DefeatCount++;
                }
                else if (attackResult == "Hit")
                {
                    actionLogText = "I attacked the enemy. The enemy is still alive.";
                }
                else if (attackResult == "Corpse")
                {
                    actionLogText = "I tried to attack it, but it was already dead.";
                }
                else if (attackResult == "Miss")
                {
                    // �܂��Ȃ�
                }
                else Debug.LogWarning($"�U�����ʂɑΉ����鏈���������B: {attackResult}");

                // �U���̃A�j���[�V�����̍Đ��I����҂B
                await UniTask.WaitForSeconds(2.0f, cancellationToken: token);
            }
            else
            {
                actionLogText = "There are no enemies around to attack.";
            }

            // �U���̌��ʂ��s�����O�ɒǉ��B
            if (TryGetComponent(out ActionLog log)) log.Add(actionLogText);
        }

        string ApplyDamage(Character target, string weapon, int damage)
        {
            if (target == null) return "Miss";

            // Defeat(���j����)�AHit(��������������)�ACorpse(���Ɏ���ł���)�AMiss(������Ȃ�����)
            return target.Damage(_adventurer.ID, weapon, damage, _adventurer.Coords);
        }
    }
}
