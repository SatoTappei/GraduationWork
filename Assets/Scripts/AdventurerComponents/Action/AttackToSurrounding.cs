using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class AttackToSurrounding : SurroundingAction
    {
        Adventurer _adventurer;
        Blackboard _blackboard;
        Animator _animator;
        ActionLog _actionLog;
        LineApply _line;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _blackboard = GetComponent<Blackboard>();
            _animator = GetComponentInChildren<Animator>();
            _actionLog = GetComponent<ActionLog>();
            _line = GetComponent<LineApply>();
        }

        public async UniTask AttackAsync<TTarget>(CancellationToken token) where TTarget : IDamageable
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            // 周囲に攻撃可能な対象が居ない場合。
            if (!TryGetTarget<TTarget>(out Actor target))
            {
                _actionLog.Add("There are no enemies around to attack.");
                return;
            }

            // 攻撃する前に目標に向く。
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Attack");
            _line.ShowLine(RequestLineType.Attack);

            // ダメージを与える。
            string result = string.Empty;
            if (target is Character targetCharacter)
            {
                // Defeat(撃破した)、Hit(当たったが生存)、Corpse(既に死んでいる)、Miss(当たらなかった)
                result = targetCharacter.Damage(_blackboard.Attack, _adventurer.Coords);
            }
            else
            {
                result = "Miss";
            }

            // 攻撃結果を行動ログに追加。
            if (result == "Defeat")
            {
                _actionLog.Add("I attacked the enemy. I defeated the enemy.");
            }
            else if (result == "Hit")
            {
                _actionLog.Add("I attacked the enemy. The enemy is still alive.");
            }
            else if (result == "Corpse")
            {
                _actionLog.Add("I tried to attack it, but it was already dead.");
            }
            else if (result == "Miss")
            {
                // まだない
            }
            else
            {
                Debug.LogWarning($"攻撃結果に対応する処理が無い。スペルミス？: {result}");
            }

            // 攻撃のアニメーションの再生終了を待つ。
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            // 敵を撃破した場合、台詞とログを表示し、撃破カウントを増やす。
            if (result == "Defeat")
            {
                _line.ShowLine(RequestLineType.DefeatEnemy);
                GameLog.Add($"システム", $"{_blackboard.DisplayName}が敵を倒した。", GameLogColor.White);

                _blackboard.DefeatCount++;
            }
        }
    }
}