using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class MovementToDirection : Movement
    {
        public async UniTask MoveAsync(Vector2Int direction, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            CreatePath(direction);
            await MoveNextCellAsync(token);
        }

        void CreatePath(Vector2Int direction)
        {
            // 隣接していないセルには移動しないようにする。
            int x = System.Math.Sign(direction.x);
            int y = System.Math.Sign(direction.y);
            Vector2Int targetCoords = Coords + new Vector2Int(x, y);

            // 隣のセルへの移動なので、経路探索せずに手動で経路を設定。
            Cell cell = DungeonManager.GetCell(targetCoords);
            MovementPath.CreateManually(direction.ToString(), cell);
        }
    }
}
