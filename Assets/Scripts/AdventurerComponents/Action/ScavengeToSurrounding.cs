using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class ScavengeToSurrounding : SurroundingAction
    {
        Blackboard _blackboard;
        Animator _animator;
        ItemInventory _inventory;
        LineApply _line;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _animator = GetComponentInChildren<Animator>();
            _line = GetComponent<LineApply>();
        }

        public async UniTask ScavengeAsync(CancellationToken token)
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            // 漁る際は宝箱を優先する。
            if (TryGetTarget<Treasure>(out Actor target) || TryGetTarget<IScavengeable>(out target)) { }

            // 周囲に漁るものが無い場合。
            if (target == null)
            {
                _actionLog.Add("There are no areas around that can be scavenged.");
                return;
            }

            // 漁る前に目標に向く。
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Scav");

            // アイテムを漁る。
            Item foundItem;
            if (target is IScavengeable targetEntity)
            {
                foundItem = targetEntity.Scavenge();
            }

            _inventory.Add(foundItem);

            // 何かしら入手した場合、ログに表示。
            if (foundItem == null)
            {
                // まだない
            }
            else if (foundItem.Name.English == "Treasure")
            {
                GameLog.Add("システム", $"{_blackboard.DisplayName}が宝物を入手。", GameLogColor.White);
            }
            else
            {
                GameLog.Add("システム", $"{_blackboard.DisplayName}がアイテムを入手。", GameLogColor.White);
            }

            // 漁った結果を行動ログに追加。
            if (foundItem == null)
            {
                _actionLog.Add("I scavenged the surrounding boxes. There was nothing in them.");
            }
            else if (foundItem.Name.English == "Treasure")
            {
                _actionLog.Add("I scavenged the surrounding treasure chests. I got the treasure.");
            }
            else
            {
                _actionLog.Add($"I scavenged the surrounding boxes. I got the {foundItem}.");
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

            // 宝箱を獲得した場合

            // アニメーションなどの演出を待つ。
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            {
                _animator.Play("Scav");

                // 漁った結果に応じてゲーム進行ログと台詞に追加する内容が異なる。
                string gameLogText = string.Empty;
                RequestLineType lineType = RequestLineType.None;

                // アイテムを取得。
                string foundItem = ApplyScavenge(target as IScavengeable);
                if (foundItem == "Treasure")
                {
                    gameLogText = $"{_blackboard.DisplayName}が宝物を入手。";
                    actionLogText = "I scavenged the surrounding treasure chests. I got the treasure.";
                    lineType = RequestLineType.GetTreasureSuccess;

                    _blackboard.TreasureCount++;
                }
                else if (foundItem == "Empty")
                {
                    actionLogText = "I scavenged the surrounding boxes. There was nothing in them.";
                    lineType = RequestLineType.GetItemFailure;
                }
                else
                {
                    gameLogText = $"{_blackboard.DisplayName}がアイテムを入手。";
                    actionLogText = $"I scavenged the surrounding boxes. I got the {foundItem}.";
                    lineType = RequestLineType.GetItemSuccess;
                }

                // 表示する内容がある場合はゲーム進行ログに表示。
                if (gameLogText != string.Empty)
                {
                    GameLog.Add("システム", gameLogText, GameLogColor.White);
                }

                // 漁った結果に応じた台詞を表示。
                if (TryGetComponent(out LineApply line)) line.ShowLine(lineType);

                // アニメーションなどの演出を待つ。
                await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);
            }
            else
            {
                actionLogText = "There are no areas around that can be scavenged.";
            }

            // 漁った結果を行動ログに追加。
            if (TryGetComponent(out ActionLog log)) log.Add(actionLogText);
        }

        string ApplyScavenge(IScavengeable target)
        {
            if (target == null) return "Empty";

            Item result = target.Scavenge();
            if (result != null)
            {
                // 取得したアイテムをインベントリに追加。
                if (TryGetComponent(out ItemInventory inventory)) inventory.Add(result);

                return result.Name.English;
            }
            else
            {
                return "Empty";
            }
        } 
    }
}
