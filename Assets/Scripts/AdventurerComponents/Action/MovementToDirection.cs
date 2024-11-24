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
            // �אڂ��Ă��Ȃ��Z���ɂ͈ړ����Ȃ��悤�ɂ���B
            int x = System.Math.Sign(direction.x);
            int y = System.Math.Sign(direction.y);
            Vector2Int targetCoords = Coords + new Vector2Int(x, y);

            // �ׂ̃Z���ւ̈ړ��Ȃ̂ŁA�o�H�T�������Ɏ蓮�Ōo�H��ݒ�B
            Cell cell = DungeonManager.GetCell(targetCoords);
            MovementPath.CreateManually(direction.ToString(), cell);
        }
    }
}
