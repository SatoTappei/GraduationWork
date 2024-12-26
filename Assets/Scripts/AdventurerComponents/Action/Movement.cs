using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    // �ڕW�Ɍ������Ĉړ�����ꍇ�ƁA�C�ӂ̕����Ɉړ�����ꍇ�Ōo�H�̍쐬���@���قȂ�B
    // ���̂��߁A�h���N���X�Ōo�H�̍쐬���s���A���̃N���X�̈ړ��������Ăяo���B
    public class Movement : BaseAction
    {
        DungeonManager _dungeonManager;
        MovementPath _movementPath;
        Adventurer _adventurer;
        Blackboard _blackboard;
        Animator _animator;

        protected DungeonManager DungeonManager
        {
            get => _dungeonManager;
        }
        protected MovementPath MovementPath
        {
            get => _movementPath;
        }
        protected Vector2Int Coords 
        { 
            get => _blackboard.Coords; 
            private set => _blackboard.Coords = value; 
        }
        protected Vector2Int Direction 
        { 
            get => _blackboard.Direction; 
            private set => _blackboard.Direction = value; 
        }

        void Awake()
        {
            DungeonManager.TryFind(out _dungeonManager);
            _adventurer = GetComponent<Adventurer>();
            _blackboard = GetComponent<Blackboard>();
            _movementPath = GetComponent<MovementPath>();
            _animator = GetComponentInChildren<Animator>();
        }

        // �h���N���X����Ăяo���ړ������B
        protected async UniTask MoveNextCellAsync(CancellationToken token)
        {
            // �V���A���C�Y���Ă��ǂ��B
            const float MoveSpeed = 1.0f;
            const float RotateSpeed = 4.0f;

            Vector2Int nextCellCoords = _movementPath.Current.Coords;
            Vector3 nextCellPosition = _movementPath.Current.Position;

            // �����̒l�����̃Z���̕����ɍX�V�B
            Direction = nextCellCoords - Coords;

            // �ڂ̑O�ɔ�������ꍇ�͊J����B
            Vector2Int forwardCoords = Coords + Direction;
            if (TryGetComponent(out DoorOpenApply door))
            {
                door.Open(forwardCoords);
            }

            string logText = string.Empty;
            float speedMag = _blackboard.SpeedMagnification;

            // �ʍs�\�ȃZ���̏ꍇ�͈ړ��A�s�\�̏ꍇ�͂��̃Z���̕����ɉ�]�̂ݍs���B
            if (_movementPath.Current.IsPassable())
            {
                _animator.SetFloat("Speed", speedMag);
                _animator.Play("Walk");

                // ���W�̒l�����̃Z���̍��W�ɍX�V�B
                _dungeonManager.RemoveActorOnCell(Coords, _adventurer);
                Coords = nextCellCoords;
                _dungeonManager.AddActorOnCell(Coords, _adventurer);

                await (TranslateAsync(MoveSpeed * speedMag, nextCellPosition, token),
                    RotateAsync(RotateSpeed * speedMag, nextCellPosition, token));

                // �ړ��ɐ����������Ƃ��L�^�B
                logText = $"Successfully moved to the {GetDirectionName()}.";

                // �T�������Z���Ƃ��čX�V�B
                if (TryGetComponent(out ExploreRecord record)) record.IncreaseCount(Coords);

                _animator.Play("Idle");
                _animator.SetFloat("Speed", 1.0f);
            }
            else
            {
                await RotateAsync(RotateSpeed * speedMag, nextCellPosition, token);

                // �ړ��Ɏ��s�������Ƃ��L�^�B
                logText = $"Failed to move to the {GetDirectionName()}. Cannot move in this direction.";
            }

            // �ړ��̌��ʂ��s�����O�ɒǉ��B
            if (TryGetComponent(out ActionLog log)) log.Add(logText);

            // ���̃Z�����X�V�B
            _movementPath.HeadingNext();
        }

        // �㉺���E�̌����𓌐���k�ɑΉ�������B
        string GetDirectionName()
        {
            if (Direction == Vector2Int.up) return "north";
            if (Direction == Vector2Int.down) return "south";
            if (Direction == Vector2Int.left) return "west";
            if (Direction == Vector2Int.right) return "east";

            return string.Empty;
        }
    }
}
