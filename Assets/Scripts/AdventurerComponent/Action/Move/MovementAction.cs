using Cysharp.Threading.Tasks;
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
        Animator _animator;

        protected Adventurer Adventurer => _adventurer;
        protected MovementPath MovementPath => _movementPath;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _movementPath = GetComponent<MovementPath>();
            _animator = GetComponentInChildren<Animator>();
        }

        // �h���N���X����Ăяo���ړ������B
        protected async UniTask<ActionResult> MoveAsync(CancellationToken token)
        {
            // �s���J�n�̃^�C�~���O�Ŏ��S���Ă����ꍇ�B
            if (_adventurer.Status.CurrentHp <= 0)
            {
                return new ActionResult(
                    "Move",
                    "Failure",
                    $"Died.",
                    _adventurer.Coords,
                    _adventurer.Direction
                );
            }

            Vector2Int nextCoords = _movementPath.GetCurrent().Coords;
            Vector2Int nextDirection = nextCoords - _adventurer.Coords;

            // �ڂ̑O�ɔ�������ꍇ�͊J����B
            // �_���W�����������A���𐶐�����Z���� ��(8),��(2),��(4),�E(6) �Ō������w�肵�Ă���B
            if ("2468".Contains(Blueprint.Doors[nextCoords.y][nextCoords.x]))
            {
                foreach (Actor actor in DungeonManager.GetActors(nextCoords))
                {
                    if (actor.ID == "Door" && actor is Door door)
                    {
                        door.Interact(_adventurer);
                    }
                }
            }

            // �ʍs�\�ȃZ���̏ꍇ�͈ړ��A�s�\�̏ꍇ�͂��̃Z���̕����ɉ�]�̂ݍs���B
            if (_movementPath.GetCurrent().IsPassable())
            {
                _animator.SetFloat("Speed", _adventurer.Status.TotalSpeed);
                _animator.Play("Walk");

                await (
                    TranslateAsync(
                        _adventurer.Status.TotalSpeed,
                        _movementPath.GetCurrent().Position, 
                        token
                    ),
                    RotateAsync(
                        _adventurer.Status.TotalRotateSpeed,
                        _movementPath.GetCurrent().Position, 
                        token
                    )
                );

                _animator.Play("Idle");
                _animator.SetFloat("Speed", 1.0f);
            }
            else
            {
                await RotateAsync(
                    _adventurer.Status.TotalRotateSpeed, 
                    _movementPath.GetCurrent().Position, 
                    token
                );
            }

            // �s�����O�ɒǉ����邽�߁A�㉺���E�̌����𓌐���k�ɑΉ�������B
            string directionName = "surrounding";
            if (nextDirection == Vector2Int.up) directionName = "north";
            else if (nextDirection == Vector2Int.down) directionName = "south";
            else if (nextDirection == Vector2Int.left) directionName = "west";
            else if (nextDirection == Vector2Int.right) directionName = "east";

            // �ړ����ʂ�Ԃ��B
            if (_movementPath.GetCurrent().IsPassable())
            {
                return new ActionResult(
                    "Move",
                    "Success",
                    $"Successfully moved to the {directionName}.",
                    nextCoords,
                    nextDirection,
                    nextCoords
                );
            }
            else
            {
                return new ActionResult(
                    "Move",
                    "Failure",
                    $"Failed to move to the {directionName}. Cannot move in this direction.",
                    _adventurer.Coords,
                    nextDirection
                );
            }
        }
    }
}