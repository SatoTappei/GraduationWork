using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class MovementToTarget : Movement
    {
        public async UniTask MoveAsync(string targetID, CancellationToken token)
        {
            CreatePath(targetID);
            await MoveNextCellAsync(token);
        }

        void CreatePath(string targetID)
        {
            // 同じ目標に対して移動し続けている限り、再度経路探索はしない。
            // ワープなどの強制的に位置を移動させるギミックを作る場合は破綻するので注意。
            if (MovementPath.Target == targetID) return;

            if (targetID == "Treasure")
            {
                Cell cell = DungeonManager.GetCells(targetID).FirstOrDefault();
                Actor treasure = cell.GetActors().Where(a => a.ID == targetID).FirstOrDefault();
                // 宝箱のマスへは経路探索が出来ないので、正面の位置までの経路探索。
                MovementPath.Finding(targetID, Coords, treasure.Coords + treasure.Direction);

            }
            else if (targetID == "Entrance")
            {
                Cell cell = DungeonManager.GetCells(targetID).FirstOrDefault();
                MovementPath.Finding(targetID, Coords, cell.Coords);
            }
            else Debug.LogWarning($"対応する目標が存在しないため経路探索が出来ない。: {targetID}");
        }
    }
}
