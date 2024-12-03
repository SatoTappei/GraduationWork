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
        DungeonManager _dungeonManager;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _animator = GetComponentInChildren<Animator>();
            DungeonManager.TryFind(out _dungeonManager);
        }

        public async UniTask ScavengeAsync(CancellationToken token)
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            token.ThrowIfCancellationRequested();

            // 漁った結果によって行動ログに追加する内容が異なる。
            string actionLogText = string.Empty;

            // 漁る際は宝箱を優先する。
            if (TryGetTarget<Treasure>(out Actor target) || TryGetTarget<IScavengeable>(out target))
            {
                // 漁る前に目標に向く。
                Vector3 targetPosition = _dungeonManager.GetCell(target.Coords).Position;
                await RotateAsync(RotateSpeed, targetPosition, token);

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
                if (gameLogText != string.Empty && UiManager.TryFind(out UiManager ui))
                {
                    ui.AddLog(gameLogText);
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
