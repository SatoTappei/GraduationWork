using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game.EnemyComponent
{
    public class MovementAction : BaseAction
    {
        [SerializeField] float _moveSpeed = 1.0f;
        [SerializeField] float _rotateSpeed = 4.0f;

        Enemy _enemy;
        Animator _animator;

        void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _animator = GetComponentInChildren<Animator>();
        }

        public async UniTask<ActionResult> MoveAsync(Vector2Int direction, CancellationToken token)
        {
            Cell nextCell = DungeonManager.GetCell(_enemy.Coords + direction);
            Vector2Int nextDirection = nextCell.Coords - _enemy.Coords;

            // �ʍs�\�ȃZ���̏ꍇ�͈ړ��A�s�\�̏ꍇ�͂��̃Z���̕����ɉ�]�̂ݍs���B
            if (nextCell.IsPassable())
            {
                _animator.Play("Walk");

                await (
                    TranslateAsync(_moveSpeed, nextCell.Position, token),
                    RotateAsync(_rotateSpeed, nextCell.Position, token)
                );

                _animator.Play("Idle");
            }
            else
            {
                await RotateAsync(_rotateSpeed, nextCell.Position, token);
            }

            // �ړ����ʂ�Ԃ��B
            if (nextCell.IsPassable())
            {
                return new ActionResult(
                    "Move",
                    "Success",
                    string.Empty,
                    nextCell.Coords,
                    nextDirection
                );
            }
            else
            {
                return new ActionResult(
                    "Move",
                    "Failure",
                    string.Empty,
                    _enemy.Coords,
                    nextDirection
                );
            }
        }
    }
}
