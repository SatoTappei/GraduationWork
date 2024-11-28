using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class EnemyAttack : SurroundingAction
    {
        [SerializeField] float _animationLength = 2.0f;
        [SerializeField] float _rotateSpeed = 4.0f;
        [SerializeField] int _damage = 10;
        [SerializeField] string _weapon = "�p���`";

        DungeonManager _dungeonManager;
        Enemy _enemy;
        Animator _animator;

        void Awake()
        {
            _dungeonManager = DungeonManager.Find();
            _enemy = GetComponent<Enemy>();
            _animator = GetComponentInChildren<Animator>();
        }

        public async UniTask AttackAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            // ���͂ɍU���\�ȑΏۂ�����ꍇ�͍U���B
            if (TryGetTarget<Adventurer>(out Actor target))
            {
                // �U������O�ɖڕW�Ɍ����B
                Vector3 targetPosition = _dungeonManager.GetCell(target.Coords).Position;
                await RotateAsync(_rotateSpeed, targetPosition, token);

                _animator.Play("Attack");

                // �_���[�W��^����B
                ApplyDamage(target as IDamageable, _weapon, _damage);

                // �A�j���[�V�����̍Đ��I����҂B
                await UniTask.WaitForSeconds(_animationLength, cancellationToken: token);
            }
        }

        string ApplyDamage(IDamageable target, string weapon, int damage)
        {
            if (target == null) return "Miss";

            // Defeat(���j����)�AHit(��������������)�ACorpse(���Ɏ���ł���)�AMiss(������Ȃ�����)
            return target.Damage(_enemy.ID, weapon, damage, _enemy.Coords);
        }
    }
}
