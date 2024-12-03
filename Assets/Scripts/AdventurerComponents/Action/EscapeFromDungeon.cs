using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class EscapeFromDungeon : BaseAction
    {
        Adventurer _adventurer;
        Blackboard _blackboard;
        SubGoalPath _subGoalPath;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _blackboard = GetComponent<Blackboard>();
            _subGoalPath = GetComponent<SubGoalPath>();
        }

        public async UniTask<bool> EscapeAsync(CancellationToken token)
        {
            const float AnimationLength = 1.0f * 2;

            // 最後のサブゴールをクリアした状態かつ入口に立っている場合は脱出。
            bool isLast = _subGoalPath.IsLast;
            bool isEntrance = Blueprint.Interaction[_adventurer.Coords.y][_adventurer.Coords.x] == '<';
            bool isCompleted = _subGoalPath.Current.IsCompleted();

            if (!(isLast && isEntrance && isCompleted)) return false;

            // 脱出の演出。
            Animator animator = GetComponentInChildren<Animator>();
            animator.Play("Jump");

            // 脱出時の台詞。
            if (TryGetComponent(out LineApply line)) line.ShowLine(RequestLineType.Goal);

            // ゲーム進行ログに表示。
            UiManager.TryFind(out UiManager uiManager);
            uiManager.AddLog($"{_blackboard.DisplayName}がダンジョンから脱出した。");

            // 演出の終了を待つ。
            await UniTask.WaitForSeconds(AnimationLength, cancellationToken: token);

            // セルから削除。
            TryGetComponent(out Adventurer adventurer);
            DungeonManager.TryFind(out DungeonManager dungeonManager);
            dungeonManager.RemoveActorOnCell(adventurer.Coords, adventurer);

            return true;
        }
    }
}