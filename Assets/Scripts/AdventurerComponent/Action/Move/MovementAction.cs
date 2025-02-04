using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Game
{
    // 目標に向かって移動する場合と、任意の方向に移動する場合で経路の作成方法が異なる。
    // そのため、派生クラスで経路の作成を行い、このクラスの移動処理を呼び出す。
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

        // 派生クラスから呼び出す移動処理。
        protected async UniTask<ActionResult> MoveAsync(CancellationToken token)
        {
            // 行動開始のタイミングで死亡していた場合。
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

            // 目の前に扉がある場合は開ける。
            // ダンジョン生成時、扉を生成するセルは 上(8),下(2),左(4),右(6) で向きを指定している。
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

            // 通行可能なセルの場合は移動、不可能の場合はそのセルの方向に回転のみ行う。
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

            // 行動ログに追加するため、上下左右の向きを東西南北に対応させる。
            string directionName = "surrounding";
            if (nextDirection == Vector2Int.up) directionName = "north";
            else if (nextDirection == Vector2Int.down) directionName = "south";
            else if (nextDirection == Vector2Int.left) directionName = "west";
            else if (nextDirection == Vector2Int.right) directionName = "east";

            // 移動結果を返す。
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