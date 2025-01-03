using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class EnemyMovement : BaseAction
    {
        [SerializeField] float _moveSpeed = 1.0f;
        [SerializeField] float _rotateSpeed = 4.0f;

        Enemy _enemy;
        EnemyBlackboard _blackboard;
        Animator _animator;

        private Vector2Int Coords
        {
            get => _blackboard.Coords;
            set => _blackboard.Coords = value;
        }
        private Vector2Int Direction
        {
            get => _blackboard.Direction;
            set => _blackboard.Direction = value;
        }

        void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _blackboard = GetComponent<EnemyBlackboard>();
            _animator = GetComponentInChildren<Animator>();
        }

        public async UniTask MoveAsync(Vector2Int direction, CancellationToken token)
        {
            // �����̒l�����̃Z���̕����ɍX�V�B
            Cell nextCell = DungeonManager.GetCell(Coords + direction);
            Direction = nextCell.Coords - Coords;

            // �ʍs�\�ȃZ���̏ꍇ�͈ړ��A�s�\�̏ꍇ�͂��̃Z���̕����ɉ�]�̂ݍs���B
            if (nextCell.IsPassable())
            {
                _animator.Play("Walk");

                // ���W�̒l�����̃Z���̍��W�ɍX�V�B
                DungeonManager.RemoveActor(Coords, _enemy);
                Coords = nextCell.Coords;
                DungeonManager.AddActor(Coords, _enemy);

                await (TranslateAsync(_moveSpeed, nextCell.Position, token),
                    RotateAsync(_rotateSpeed, nextCell.Position, token));

                _animator.Play("Idle");
            }
            else
            {
                await RotateAsync(_rotateSpeed, nextCell.Position, token);
            }
        }
    }
}
