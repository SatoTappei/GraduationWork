using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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

        public async UniTask<string> PlayAsync(CancellationToken token)
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            // 漁る際は宝箱を優先する。
            if (TryGetTarget<Treasure>(out Actor target) || TryGetTarget<IScavengeable>(out target)) { }

            // 周囲に漁るものが無い場合。
            if (target == null)
            {
                return "There are no areas around that can be scavenged.";
            }

            // 漁る前に目標に向く。
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
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
            else if (foundItem.Name.English == "Treasure")
            {
                GameLog.Add(
                    "システム", 
                    $"{_adventurer.AdventurerSheet.DisplayName}が宝物を入手。", 
                    GameLogColor.White
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
            else if (foundItem.Name.English == "Treasure")
            {
                _line.ShowLine(RequestLineType.GetTreasureSuccess);
            }
            else
            {
                _line.ShowLine(RequestLineType.GetItemSuccess);
            }

            // 宝箱を獲得した場合、宝箱カウントを増やす。
            if (foundItem != null && foundItem.Name.English == "Treasure")
            {
                _adventurer.Status.TreasureCount++;
            }

            // アニメーションなどの演出を待つ。
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            // 漁った結果を返す。
            if (foundItem == null)
            {
                return "I scavenged the surrounding boxes. There was nothing in them.";
            }
            else if (foundItem.Name.English == "Treasure")
            {
                return "I scavenged the surrounding treasure chests. I got the treasure.";
            }
            else
            {
                return $"I scavenged the surrounding boxes. I got the {foundItem}.";
            }
        } 
    }
}
