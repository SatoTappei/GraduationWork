using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game.EnemyComponent
{
    public class AttackAction : SurroundingAction
    {
        [SerializeField] float _animationLength = 2.0f;
        [SerializeField] float _rotateSpeed = 4.0f;
        [SerializeField] int _damage = 10;

        Enemy _enemy;
        Animator _animator;

        void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _animator = GetComponentInChildren<Animator>();
        }

        public async UniTask<ActionResult> AttackAsync(CancellationToken token)
        {
            // ���͂ɍU���\�ȑΏۂ����Ȃ��ꍇ�B
            if (!TryGetTarget<Adventurer>(out Actor target))
            {
                return new ActionResult(
                    string.Empty,
                    _enemy.Coords,
                    _enemy.Direction
                );
            }

            // �U������O�ɖڕW�Ɍ����B
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(_rotateSpeed, targetPosition, token);

            _animator.Play("Attack");

            if (target != null && TryGetComponent(out IDamageable damage))
            {
                // Defeat(���j����)�AHit(��������������)�ACorpse(���Ɏ���ł���)�AMiss(������Ȃ�����)
                damage.Damage(_damage, _enemy.Coords);
            }

            // �A�j���[�V�����̍Đ��I����҂B
            await UniTask.WaitForSeconds(_animationLength, cancellationToken: token);

            return new ActionResult(
                string.Empty,
                _enemy.Coords,
                _enemy.Direction
            );
        }
    }
}
