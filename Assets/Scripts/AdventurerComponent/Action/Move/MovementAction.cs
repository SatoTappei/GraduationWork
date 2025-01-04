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
        protected async UniTask<string> MoveNextAsync(CancellationToken token)
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;

            // 向きの値を次のセルの方向に更新。
            _adventurer.SetDirection(_movementPath.GetCurrent().Coords - _adventurer.Coords);

            // 目の前に扉がある場合は開ける。
            // ダンジョン生成時、扉を生成するセルは 上(8),下(2),左(4),右(6) で向きを指定している。
            Vector2Int forwardCoords = _adventurer.Coords + _adventurer.Direction;
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

            // 座標の値を次のセルの座標に更新。
            DungeonManager.RemoveActor(_adventurer.Coords, _adventurer);
            _adventurer.SetCoords(_movementPath.GetCurrent().Coords);
            DungeonManager.AddActor(_adventurer.Coords, _adventurer);

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
                        RotateSpeed * _adventurer.Status.SpeedMagnification,
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
                    RotateSpeed * _adventurer.Status.SpeedMagnification, 
                    _movementPath.GetCurrent().Position, 
                    token
                );
            }

            // 行動ログに追加するため、上下左右の向きを東西南北に対応させる。
            string direction = "surrounding";
            if (_adventurer.Direction == Vector2Int.up) direction = "north";
            else if (_adventurer.Direction == Vector2Int.down) direction = "south";
            else if (_adventurer.Direction == Vector2Int.left) direction = "west";
            else if (_adventurer.Direction == Vector2Int.right) direction = "east";

            // 移動出来た場合、探索したセルとして更新。
            if (_movementPath.GetCurrent().IsPassable())
            {
                _adventurer.Status.ExploreRecord.Increase(_adventurer.Coords);
            }

            // 移動結果を返す。
            if (_movementPath.GetCurrent().IsPassable())
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
