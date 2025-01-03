using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class EscapeAction : BaseAction
    {
        SubGoalPath _subGoalPath;

        void Awake()
        {
            _subGoalPath = GetComponent<SubGoalPath>();
        }

        public async UniTask<bool> PlayAsync(CancellationToken token)
        {
            const float PlayTime = 1.0f * 2;

            // サブコールが設定されていない場合。
            if (_subGoalPath.GetCurrent() == null) return false;

            // 現在のサブゴールが「ダンジョンの入口に戻る。」かつ、サブゴールが完了したかチェック。
            bool isLast = _subGoalPath.GetCurrent().Text.Japanese == ReturnToEntrance.JapaneseText;
            bool isCompleted = _subGoalPath.GetCurrent().IsCompleted();
            if (!(isLast && isCompleted)) return false;

            // 脱出の演出。
            Animator animator = GetComponentInChildren<Animator>();
            animator.Play("Jump");

            // 脱出時の台詞。
            TryGetComponent(out LineDisplayer line);
            line.ShowLine(RequestLineType.Goal);

            // ゲーム進行ログに表示。
            TryGetComponent(out Adventurer adventurer);
            GameLog.Add(
                $"システム", 
                $"{adventurer.AdventurerSheet.DisplayName}がダンジョンから脱出した。", 
                GameLogColor.Yellow
            );

            // 演出の終了を待つ。
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            return true;
        }
    }
}