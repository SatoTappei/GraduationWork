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

        public async UniTask MoveAsync(Vector2Int direction, CancellationToken token)
        {
            // �����̒l�����̃Z���̕����ɍX�V�B
            Cell nextCell = DungeonManager.GetCell(_enemy.Coords + direction);
            _enemy.SetDirection(nextCell.Coords - _enemy.Coords);

            // �ʍs�\�ȃZ���̏ꍇ�͈ړ��A�s�\�̏ꍇ�͂��̃Z���̕����ɉ�]�̂ݍs���B
            if (nextCell.IsPassable())
            {
                _animator.Play("Walk");

                // ���W�̒l�����̃Z���̍��W�ɍX�V�B
                DungeonManager.RemoveActor(_enemy.Coords, _enemy);
                _enemy.SetCoords(nextCell.Coords);
                DungeonManager.AddActor(_enemy.Coords, _enemy);

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
        }
    }
}
