using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class EscapeFromDungeon : BaseAction
    {
        Blackboard _blackboard;
        SubGoalPath _subGoalPath;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _subGoalPath = GetComponent<SubGoalPath>();
        }

        public async UniTask<bool> EscapeAsync(CancellationToken token)
        {
            const float AnimationLength = 1.0f * 2;

            // 現在のサブゴールが「ダンジョンの入口に戻る。」かつ、サブゴールが完了したかチェック。
            bool isLast = _subGoalPath.GetCurrent().Text.Japanese == ReturnToEntrance.JapaneseText;
            bool isCompleted = _subGoalPath.GetCurrent().IsCompleted();
            if (!(isLast && isCompleted)) return false;

            // 脱出の演出。
            Animator animator = GetComponentInChildren<Animator>();
            animator.Play("Jump");

            // 脱出時の台詞。
            if (TryGetComponent(out LineApply line)) line.ShowLine(RequestLineType.Goal);

            // ゲーム進行ログに表示。
            GameLog.Add($"システム", $"{_blackboard.DisplayName}がダンジョンから脱出した。", GameLogColor.Yellow);

            // 演出の終了を待つ。
            await UniTask.WaitForSeconds(AnimationLength, cancellationToken: token);

            // セルから削除。
            TryGetComponent(out Adventurer adventurer);
            DungeonManager.RemoveActorOnCell(adventurer.Coords, adventurer);

            return true;
        }
    }
}