using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VTNConnect;

namespace Game
{
    public class AttackAction : SurroundingAction
    {
        Adventurer _adventurer;
        Animator _animator;
        LineDisplayer _line;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _animator = GetComponentInChildren<Animator>();
            _line = GetComponent<LineDisplayer>();
        }

        public async UniTask<ActionResult> PlayAsync<TTarget>(CancellationToken token) where TTarget : class
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            // 周囲に攻撃可能な対象が居ない場合。
            if (!TryGetTarget<TTarget>(out Actor target))
            {
                return new ActionResult(
                    "Attack",
                    "Miss",
                    "There are no enemies around to attack.",
                    _adventurer.Coords,
                    _adventurer.Direction
                );
            }

            // 攻撃する前に目標に向く。
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Attack");
            _line.ShowLine(RequestLineType.Attack);

            // ダメージを与える。
            string result;
            if (target != null && target.TryGetComponent(out IDamageable damage))
            {
                // Defeat(撃破した)、Hit(当たったが生存)、Corpse(既に死んでいる)、Miss(当たらなかった)
                result = damage.Damage(_adventurer.Status.TotalAttack, _adventurer.Coords);
            }
            else
            {
                result = "Miss";
            }

            // 攻撃のアニメーションの再生終了を待つ。
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            // 敵を撃破した場合、台詞とログを表示し、撃破カウントを増やす。
            if (result == "Defeat")
            {
                _line.ShowLine(RequestLineType.DefeatEnemy);

                GameLog.Add(
                    $"システム", 
                    $"敵を倒した。",
                    LogColor.White,
                    _adventurer.AdventurerSheet.Number
                );

                _adventurer.Status.DefeatCount++;
            }

            // 攻撃結果ごとの行動ログに追加する文章。
            string actionLog;
            if (result == "Defeat")
            {
                actionLog = "I attacked the enemy. I defeated the enemy.";
            }
            else if (result == "Hit")
            {
                actionLog = "I attacked the enemy. The enemy is still alive.";
            }
            else if (result == "Corpse")
            {
                actionLog = "I tried to attack it, but it was already dead.";
            }
            else if (result == "Miss")
            {
                actionLog = "I tried to attack it, but it was miss.";
            }
            else
            {
                Debug.LogWarning($"攻撃結果に対応する処理が無い。スペルミス？: {result}");
                
                actionLog = "I tried to attack it.";
            }

            // 敵を倒した場合はイベントを送信。
            if (result == "Defeat")
            {
                EventData eventData = new EventData(EventDefine.DefeatBomb);
                VantanConnect.SendEvent(eventData);
            }

            return new ActionResult(
                "Attack",
                result,
                actionLog,
                _adventurer.Coords,
                _adventurer.Direction
            );
        }
    }
}