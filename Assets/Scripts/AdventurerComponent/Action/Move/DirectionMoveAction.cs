using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class DirectionMoveAction : MovementAction
    {
        public async UniTask<ActionResult> PlayAsync(Vector2Int direction, CancellationToken token)
        {
            // 隣接していないセルには移動しないようにする。
            int x = System.Math.Sign(direction.x);
            int y = System.Math.Sign(direction.y);
            Vector2Int targetCoords = Adventurer.Coords + new Vector2Int(x, y);

            // 隣のセルへの移動なので、経路探索せずに手動で経路を設定。
            Cell cell = DungeonManager.GetCell(targetCoords);
            MovementPath.Create(direction.ToString(), cell);

            return await MoveAsync(token);
        }
    }
}
