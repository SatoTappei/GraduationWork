using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Game.ItemData;
using VTNConnect;

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

            // 漁る際の優先度は 鍵->宝箱->その他 の順。
            if (TryGetTarget<TreasureChestKey>(out Actor target) || 
                TryGetTarget<Treasure>(out target) || 
                TryGetTarget<IScavengeable>(out target)) { }

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
            string result = string.Empty;
            if (target != null && target is IScavengeable targetEntity)
            {
                result = targetEntity.Scavenge(_adventurer, out foundItem);

                if (foundItem != null) _inventory.Add(foundItem);
            }

            // 漁った結果によってログに追加する内容が変わる。
            if (result == "Get")
            {
                if (foundItem != null && foundItem.Name.English == "Artifact")
                {
                    GameLog.Add(
                        "システム",
                        $"{foundItem.Name.Japanese}を入手！",
                        LogColor.Yellow,
                        _adventurer.AdventurerSheet.Number
                    );
                }
                else if(foundItem != null)
                {
                    GameLog.Add(
                        "システム",
                        $"{foundItem.Name.Japanese}を入手。",
                        LogColor.White,
                        _adventurer.AdventurerSheet.Number
                    );
                }
            }
            else if (result == "Empty")
            {
                //
            }
            else if (result == "Lock")
            {
                GameLog.Add(
                    "システム",
                    $"宝箱の鍵を持っていない。",
                    LogColor.White,
                    _adventurer.AdventurerSheet.Number
                );
            }
            else
            {
                Debug.LogWarning($"漁った結果の値がおかしい。スペルミス？{result}");
            }

            // 手に入れたアイテムによって台詞が変わる。
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

            // 手に入れたアイテムに対応したカウントを増やす。
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

            // 漁った結果ごとの行動ログに追加する文章。
            string actionLog;
            if (result == "Get")
            {
                if (foundItem != null && foundItem.Name.English == "Artifact")
                {
                    actionLog = "I scavenged the surrounding altar. I got the legendary treasure.";
                }
                else if (foundItem != null && foundItem.Name.English == "Treasure")
                {
                    actionLog = "I scavenged the surrounding treasure chests. I got the treasure.";
                }
                else if (foundItem != null)
                {
                    actionLog = $"I scavenged the surrounding boxes. I got the {foundItem.Name.English}.";
                }
                else
                {
                    actionLog = "I scavenged the surrounding boxes. There was nothing in them.";
                }
            }
            else if (result == "Empty")
            {
                actionLog = "I scavenged the surrounding boxes. There was nothing in them.";
            }
            else if (result == "Lock")
            {
                actionLog = "I did not have the keys to open the surrounding treasure chests.";
            }
            else
            {
                Debug.LogWarning($"漁った結果の値がおかしい。スペルミス？{result}");
                actionLog = "I scavenged the surrounding boxes. There was nothing in them.";
            }

            // 入手したアイテムに対応したイベントがある場合は送信。
            EventData eventData = null;
            if (foundItem == null) { }
            else if (foundItem.Name.Japanese == "クラッカー")
            {
                eventData = new EventData(EventDefine.ActorEffect);
            }
            else if (foundItem.Name.Japanese == "壊れた罠")
            {
                eventData = new EventData(EventDefine.ReviveGimmick);
            }
            else if (foundItem.Name.Japanese == "錆びた剣")
            {
                eventData = new EventData(EventDefine.PickupItem);
            }
            else if (foundItem.Name.Japanese == "切れた電球")
            {
                eventData = new EventData(EventDefine.DarkRoom);
            }
            else if (foundItem.Name.Japanese == "ヘルメット")
            {
                eventData = new EventData(EventDefine.SummonEnemy);
            }

            if (eventData != null) VantanConnect.SendEvent(eventData);

            // 何かしらアイテムを入手出来れば成功、出来なければ失敗。
            return new ActionResult(
                "Scavenge",
                foundItem != null ? "Success" : "Failure",
                actionLog,
                _adventurer.Coords,
                targetDirection,
                _adventurer.Coords + targetDirection
            );
        } 
    }
}
