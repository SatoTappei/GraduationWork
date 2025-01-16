using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Game.ItemData;

namespace Game
{
    public class ScavengeAction : SurroundingAction
    {
        Adventurer _adventurer;
        Animator _animator;
        ItemInventory _inventory;
        LineDisplayer _line;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _animator = GetComponentInChildren<Animator>();
            _inventory = GetComponent<ItemInventory>();
            _line = GetComponent<LineDisplayer>();
        }

        public async UniTask<ActionResult> PlayAsync(CancellationToken token)
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            // 漁る際は宝箱を優先する。
            if (TryGetTarget<Treasure>(out Actor target) || TryGetTarget<IScavengeable>(out target)) { }

            // 周囲に漁るものが無い場合。
            if (target == null)
            {
                return new ActionResult(
                    "Scavenge",
                    "Failure",
                    "There are no areas around that can be scavenged.",
                    _adventurer.Coords,
                    _adventurer.Direction
                );
            }

            // 漁る前に目標に向く。
            Cell targetCell = DungeonManager.GetCell(target.Coords);
            Vector3 targetPosition = targetCell.Position;
            Vector2Int targetDirection = targetCell.Coords - _adventurer.Coords;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Scav");

            // アイテムを漁り、何かしらのアイテムを獲得した場合はインベントリに追加。
            Item foundItem = null;
            if (target != null && target is IScavengeable targetEntity)
            {
                foundItem = targetEntity.Scavenge();

                if (foundItem != null) _inventory.Add(foundItem);
            }

            // 何かしら入手した場合、ログに表示。
            if (foundItem == null)
            {
                // まだない
            }
            else if (foundItem.Name.English == "Artifact")
            {
                GameLog.Add(
                    "システム", 
                    $"{_adventurer.AdventurerSheet.DisplayName}がアーティファクトを入手！", 
                    GameLogColor.Yellow
                );
            }
            else
            {
                GameLog.Add(
                    "システム", 
                    $"{_adventurer.AdventurerSheet.DisplayName}がアイテムを入手。", 
                    GameLogColor.White
                );
            }

            // 漁った結果に応じた台詞。
            if (foundItem == null)
            {
                _line.ShowLine(RequestLineType.GetItemFailure);
            }
            else if (foundItem.Name.English == "Artifact")
            {
                _line.ShowLine(RequestLineType.GetArtifactSuccess);
            }
            else
            {
                _line.ShowLine(RequestLineType.GetItemSuccess);
            }

            // 漁った結果に対応したカウントを増やす。
            if (foundItem != null && foundItem.Name.English == "Artifact")
            {
                _adventurer.Status.ArtifactCount++;
            }
            if (foundItem != null && foundItem.Name.English == "Treasure")
            {
                _adventurer.Status.TreasureCount++;
            }

            // アニメーションなどの演出を待つ。
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            // 漁った結果。何かしら手に入れば成功。
            string result = foundItem != null ? "Success" : "Failure";

            // 漁った結果ごとの行動ログに追加する文章。
            string actionLog;
            if (foundItem == null)
            {
                actionLog = "I scavenged the surrounding boxes. There was nothing in them.";
            }
            else if (foundItem.Name.English == "Artifact")
            {
                actionLog = "I scavenged the surrounding altar. I got the legendary treasure.";
            }
            else if (foundItem.Name.English == "Treasure")
            {
                actionLog = "I scavenged the surrounding treasure chests. I got the treasure.";
            }
            else
            {
                actionLog = $"I scavenged the surrounding boxes. I got the {foundItem.Name.English}.";
            }

            return new ActionResult(
                "Scavenge",
                result,
                actionLog,
                _adventurer.Coords,
                targetDirection,
                _adventurer.Coords + targetDirection
            );
        } 
    }
}
