using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    // 目標に向かって移動する場合と、任意の方向に移動する場合で経路の作成方法が異なる。
    // そのため、派生クラスで経路の作成を行い、このクラスの移動処理を呼び出す。
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

        // 派生クラスから呼び出す移動処理。
        protected async UniTask MoveNextCellAsync(CancellationToken token)
        {
            // シリアライズしても良い。
            const float MoveSpeed = 1.0f;
            const float RotateSpeed = 4.0f;

            Vector2Int nextCellCoords = _movementPath.Current.Coords;
            Vector3 nextCellPosition = _movementPath.Current.Position;

            // 向きの値を次のセルの方向に更新。
            Direction = nextCellCoords - Coords;

            // 目の前に扉がある場合は開ける。
            Vector2Int forwardCoords = Coords + Direction;
            if (TryGetComponent(out DoorOpenApply door))
            {
                door.Open(forwardCoords);
            }

            string logText = string.Empty;
            float speedMag = _blackboard.SpeedMagnification;

            // 通行可能なセルの場合は移動、不可能の場合はそのセルの方向に回転のみ行う。
            if (_movementPath.Current.IsPassable())
            {
                _animator.SetFloat("Speed", speedMag);
                _animator.Play("Walk");

                // 座標の値を次のセルの座標に更新。
                _dungeonManager.RemoveActorOnCell(Coords, _adventurer);
                Coords = nextCellCoords;
                _dungeonManager.AddActorOnCell(Coords, _adventurer);

                await (TranslateAsync(MoveSpeed * speedMag, nextCellPosition, token),
                    RotateAsync(RotateSpeed * speedMag, nextCellPosition, token));

                // 移動に成功したことを記録。
                logText = $"Successfully moved to the {GetDirectionName()}.";

                // 探索したセルとして更新。
                if (TryGetComponent(out ExploreRecord record)) record.IncreaseCount(Coords);

                _animator.Play("Idle");
                _animator.SetFloat("Speed", 1.0f);
            }
            else
            {
                await RotateAsync(RotateSpeed * speedMag, nextCellPosition, token);

                // 移動に失敗したことを記録。
                logText = $"Failed to move to the {GetDirectionName()}. Cannot move in this direction.";
            }

            // 移動の結果を行動ログに追加。
            if (TryGetComponent(out ActionLog log)) log.Add(logText);

            // 次のセルを更新。
            _movementPath.HeadingNext();
        }

        // 上下左右の向きを東西南北に対応させる。
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
