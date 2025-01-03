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
            // �����ڕW�ɑ΂��Ĉړ��������Ă������A�ēx�o�H�T���͂��Ȃ��B
            // ���[�v�Ȃǂ̋����I�Ɉʒu���ړ�������M�~�b�N�����ꍇ�͔j�]����̂Œ��ӁB
            if (MovementPath.Target == targetID)
            {
                string result = await MoveNextAsync(token);
                MovementPath.SetNext();

                return result;
            }

            // �ڕW�ւ̌o�H�T���B
            if (targetID == "Treasure")
            {
                Cell cell = DungeonManager.GetCells(targetID).FirstOrDefault();
                Actor treasure = cell.GetActors().Where(a => a.ID == targetID).FirstOrDefault();
                // �󔠂̃}�X�ւ͌o�H�T�����o���Ȃ��̂ŁA���ʂ̈ʒu�܂ł̌o�H�T���B
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

                Debug.LogWarning($"�Ή�����ڕW�����݂��Ȃ����ߌo�H�T�����o���Ȃ��B: {targetID}");
            }

            // �o�H�����݂���ꍇ�͈ړ��B
            if (MovementPath.Current == null)
            {
                Debug.LogWarning("�ړ���ƂȂ�Z�������݂��Ȃ��B");

                return $"Failed to move to target position. Cannot move in this direction.";
            }
            else
            {
                string result = await MoveNextAsync(token);
                MovementPath.SetNext();

                return result;
            }
        }
    }
}
