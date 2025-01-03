using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    // �ڕW�Ɍ������Ĉړ�����ꍇ�ƁA�C�ӂ̕����Ɉړ�����ꍇ�Ōo�H�̍쐬���@���قȂ�B
    // ���̂��߁A�h���N���X�Ōo�H�̍쐬���s���A���̃N���X�̈ړ��������Ăяo���B
    public class MovementAction : BaseAction
    {
        MovementPath _movementPath;
        Adventurer _adventurer;
        Blackboard _blackboard;
        Animator _animator;
        ExploreRecord _record;

        protected MovementPath MovementPath
        {
            get => _movementPath;
        }

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _blackboard = GetComponent<Blackboard>();
            _movementPath = GetComponent<MovementPath>();
            _animator = GetComponentInChildren<Animator>();
            _record = GetComponent<ExploreRecord>();
        }

        // �h���N���X����Ăяo���ړ������B
        protected async UniTask<string> MoveNextAsync(CancellationToken token)
        {
            // �V���A���C�Y���Ă��ǂ��B
            const float RotateSpeed = 4.0f;

            // �����̒l�����̃Z���̕����ɍX�V�B
            _adventurer.SetDirection(_movementPath.Current.Coords - _adventurer.Coords);

            // �ڂ̑O�ɔ�������ꍇ�͊J����B
            // �_���W�����������A���𐶐�����Z���� ��(8),��(2),��(4),�E(6) �Ō������w�肵�Ă���B
            Vector2Int forwardCoords = Coords + Direction;
            if ("2468".Contains(Blueprint.Doors[forwardCoords.y][forwardCoords.x]))
            {
                foreach (Actor actor in DungeonManager.GetActors(forwardCoords))
                {
                    if (actor.ID == "Door" && actor is Door door)
                    {
                        door.Interact(_adventurer);
                    }
                }
            }

            // ���W�̒l�����̃Z���̍��W�ɍX�V�B
            DungeonManager.RemoveActor(Coords, _adventurer);
            Coords = _movementPath.Current.Coords;
            DungeonManager.AddActor(Coords, _adventurer);

            // �ʍs�\�ȃZ���̏ꍇ�͈ړ��A�s�\�̏ꍇ�͂��̃Z���̕����ɉ�]�̂ݍs���B
            if (_movementPath.Current.IsPassable())
            {
                _animator.SetFloat("Speed", _blackboard.SpeedMagnification);
                _animator.Play("Walk");

                await (
                    TranslateAsync(
                        _blackboard.Speed * _blackboard.SpeedMagnification,
                        _movementPath.Current.Position, 
                        token
                    ),
                    RotateAsync(
                        RotateSpeed * _blackboard.SpeedMagnification,
                        _movementPath.Current.Position, 
                        token
                    )
                );

                _animator.Play("Idle");
                _animator.SetFloat("Speed", 1.0f);
            }
            else
            {
                await RotateAsync(
                    RotateSpeed * _blackboard.SpeedMagnification, 
                    _movementPath.Current.Position, 
                    token
                );
            }

            // �s�����O�ɒǉ����邽�߁A�㉺���E�̌����𓌐���k�ɑΉ�������B
            string direction = "surrounding";
            if (Direction == Vector2Int.up) direction = "north";
            else if (Direction == Vector2Int.down) direction = "south";
            else if (Direction == Vector2Int.left) direction = "west";
            else if (Direction == Vector2Int.right) direction = "east";

            // �ړ��o�����ꍇ�A�T�������Z���Ƃ��čX�V�B
            if (_movementPath.Current.IsPassable())
            {
                _record.IncreaseCount(Coords);
            }

            // �ړ����ʂ�Ԃ��B
            if (_movementPath.Current.IsPassable())
            {
                return $"Successfully moved to the {direction}.";
            }
            else
            {
                return $"Failed to move to the {direction}. Cannot move in this direction.";
            }
        }
    }
}
