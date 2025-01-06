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
            // 周囲に攻撃可能な対象がいない場合。
            if (!TryGetTarget<Adventurer>(out Actor target))
            {
                return new ActionResult(
                    string.Empty,
                    _enemy.Coords,
                    _enemy.Direction
                );
            }

            // 攻撃する前に目標に向く。
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(_rotateSpeed, targetPosition, token);

            _animator.Play("Attack");

            if (target != null && TryGetComponent(out IDamageable damage))
            {
                // Defeat(撃破した)、Hit(当たったが生存)、Corpse(既に死んでいる)、Miss(当たらなかった)
                damage.Damage(_damage, _enemy.Coords);
            }

            // アニメーションの再生終了を待つ。
            await UniTask.WaitForSeconds(_animationLength, cancellationToken: token);

            return new ActionResult(
                string.Empty,
                _enemy.Coords,
                _enemy.Direction
            );
        }
    }
}
