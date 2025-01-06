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
            // �אڂ��Ă��Ȃ��Z���ɂ͈ړ����Ȃ��悤�ɂ���B
            int x = System.Math.Sign(direction.x);
            int y = System.Math.Sign(direction.y);
            Vector2Int targetCoords = Adventurer.Coords + new Vector2Int(x, y);

            // �ׂ̃Z���ւ̈ړ��Ȃ̂ŁA�o�H�T�������Ɏ蓮�Ōo�H��ݒ�B
            Cell cell = DungeonManager.GetCell(targetCoords);
            MovementPath.Create(direction.ToString(), cell);

            return await MoveAsync(token);
        }
    }
}
