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
        [SerializeField] string _weapon = "パンチ";

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

            // 周囲に攻撃可能な対象がいる場合は攻撃。
            if (TryGetTarget<Adventurer>(out Actor target))
            {
                // 攻撃する前に目標に向く。
                Vector3 targetPosition = _dungeonManager.GetCell(target.Coords).Position;
                await RotateAsync(_rotateSpeed, targetPosition, token);

                _animator.Play("Attack");

                // ダメージを与える。
                ApplyDamage(target as IDamageable, _weapon, _damage);

                // アニメーションの再生終了を待つ。
                await UniTask.WaitForSeconds(_animationLength, cancellationToken: token);
            }
        }

        string ApplyDamage(IDamageable target, string weapon, int damage)
        {
            if (target == null) return "Miss";

            // Defeat(撃破した)、Hit(当たったが生存)、Corpse(既に死んでいる)、Miss(当たらなかった)
            return target.Damage(_enemy.ID, weapon, damage, _enemy.Coords);
        }
    }
}
