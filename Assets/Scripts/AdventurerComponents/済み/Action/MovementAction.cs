using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
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

        // 派生クラスから呼び出す移動処理。
        protected async UniTask<string> MoveNextAsync(CancellationToken token)
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;

            // 向きの値を次のセルの方向に更新。
            _adventurer.SetDirection(_movementPath.Current.Coords - _adventurer.Coords);

            // 目の前に扉がある場合は開ける。
            // ダンジョン生成時、扉を生成するセルは 上(8),下(2),左(4),右(6) で向きを指定している。
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

            // 座標の値を次のセルの座標に更新。
            DungeonManager.RemoveActor(Coords, _adventurer);
            Coords = _movementPath.Current.Coords;
            DungeonManager.AddActor(Coords, _adventurer);

            // 通行可能なセルの場合は移動、不可能の場合はそのセルの方向に回転のみ行う。
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

            // 行動ログに追加するため、上下左右の向きを東西南北に対応させる。
            string direction = "surrounding";
            if (Direction == Vector2Int.up) direction = "north";
            else if (Direction == Vector2Int.down) direction = "south";
            else if (Direction == Vector2Int.left) direction = "west";
            else if (Direction == Vector2Int.right) direction = "east";

            // 移動出来た場合、探索したセルとして更新。
            if (_movementPath.Current.IsPassable())
            {
                _record.IncreaseCount(Coords);
            }

            // 移動結果を返す。
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
