using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class AttackAction : SurroundingAction
    {
        Adventurer _adventurer;
        Animator _animator;
        LineDisplayer _line;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _animator = GetComponentInChildren<Animator>();
            _line = GetComponent<LineDisplayer>();
        }

        public async UniTask<string> PlayAsync<TTarget>(CancellationToken token) where TTarget : IDamageable
        {
            // �V���A���C�Y���Ă��ǂ��B
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            // ���͂ɍU���\�ȑΏۂ����Ȃ��ꍇ�B
            if (!TryGetTarget<TTarget>(out Actor target))
            {
                return "There are no enemies around to attack.";
            }

            // �U������O�ɖڕW�Ɍ����B
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Attack");
            _line.ShowLine(RequestLineType.Attack);

            // �_���[�W��^����B
            string result = string.Empty;
            if (target != null && target is Character targetCharacter)
            {
                // Defeat(���j����)�AHit(��������������)�ACorpse(���Ɏ���ł���)�AMiss(������Ȃ�����)
                result = targetCharacter.Damage(_adventurer.Status.TotalAttack, _adventurer.Coords);
            }
            else
            {
                result = "Miss";
            }

            // �U���̃A�j���[�V�����̍Đ��I����҂B
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            // �G�����j�����ꍇ�A�䎌�ƃ��O��\�����A���j�J�E���g�𑝂₷�B
            if (result == "Defeat")
            {
                _line.ShowLine(RequestLineType.DefeatEnemy);

                GameLog.Add(
                    $"�V�X�e��", 
                    $"{_adventurer.AdventurerSheet.DisplayName}���G��|�����B",
                    GameLogColor.White
                );

                _adventurer.Status.DefeatCount++;
            }

            // �U�����ʂ�Ԃ��B
            if (result == "Defeat")
            {
                return "I attacked the enemy. I defeated the enemy.";
            }
            else if (result == "Hit")
            {
                return "I attacked the enemy. The enemy is still alive.";
            }
            else if (result == "Corpse")
            {
                return "I tried to attack it, but it was already dead.";
            }
            else if (result == "Miss")
            {
                // �܂��Ȃ�
                return "I tried to attack it, but it was miss.";
            }
            else
            {
                Debug.LogWarning($"�U�����ʂɑΉ����鏈���������B�X�y���~�X�H: {result}");
                return string.Empty;
            }
        }
    }
}