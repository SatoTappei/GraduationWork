using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class TargetMoveAction : MovementAction
    {
        public async UniTask<string> PlayAsync(string targetID, CancellationToken token)
        {
            // 同じ目標に対して移動し続けている限り、再度経路探索はしない。
            // ワープなどの強制的に位置を移動させるギミックを作る場合は破綻するので注意。
            if (MovementPath.Target == targetID)
            {
                await MoveNextAsync(token);
                MovementPath.SetNext();

                return;
            }

            // 目標への経路探索。
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
            else
            {
                MovementPath.Clear();

                Debug.LogWarning($"対応する目標が存在しないため経路探索が出来ない。: {targetID}");
            }

            // 経路が存在する場合は移動。
            if (MovementPath.Current != null)
            {
                await MoveNextAsync(token);
                MovementPath.SetNext();
            }
        }
    }
}
