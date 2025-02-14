using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class EscapeAction : BaseAction
    {
        [SerializeField] ParticleSystem _particle;

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

            // 現在のサブゴールが「ダンジョンの入口に戻る」かつ、サブゴールが完了したかチェック。
            bool isLast = _subGoalPath.GetCurrent().Description.Japanese == "ダンジョンの入口に戻る";
            bool isCompleted = _subGoalPath.GetCurrent().Check() == SubGoal.State.Completed;
            if (!(isLast && isCompleted)) return false;

            // 脱出の演出。
            Animator animator = GetComponentInChildren<Animator>();
            animator.Play("Jump");
            _particle.Play();

            // 脱出時の台詞。
            TryGetComponent(out LineDisplayer line);
            line.Show(RequestLineType.Goal);

            // ゲーム進行ログに表示。
            TryGetComponent(out Adventurer adventurer);
            GameLog.Add(
                $"システム", 
                $"ダンジョンから脱出した！", 
                LogColor.Yellow,
                adventurer.Sheet.DisplayID
            );

            // 演出の終了を待つ。
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            return true;
        }
    }
}