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

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _blackboard = GetComponent<Blackboard>();
            _animator = GetComponentInChildren<Animator>();
        }

        public async UniTask AttackAsync<TTarget>(CancellationToken token) where TTarget : IDamageable
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;
            const string Weapon = "パンチ";

            // 攻撃の結果によって行動ログに追加する内容が異なる。
            string actionLogText = string.Empty;

            // 周囲に攻撃可能な対象がいる場合は攻撃。
            if (TryGetTarget<TTarget>(out Actor target))
            {
                // 攻撃する前に目標に向く。
                Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
                await RotateAsync(RotateSpeed, targetPosition, token);

                _animator.Play("Attack");

                // 攻撃時の台詞。
                if (TryGetComponent(out LineApply line)) line.ShowLine(RequestLineType.Attack);

                // ダメージを与える。
                string attackResult = ApplyDamage(target as Character, Weapon, _blackboard.Attack);
                if (attackResult == "Defeat")
                {
                    // 撃破時の台詞。
                    if (line != null) line.ShowLine(RequestLineType.DefeatEnemy);

                    // ゲーム進行ログに表示。
                    GameLog.Add($"システム", $"{_blackboard.DisplayName}が敵を倒した。", GameLogColor.White);

                    actionLogText = "I attacked the enemy. I defeated the enemy.";

                    _blackboard.DefeatCount++;
                }
                else if (attackResult == "Hit")
                {
                    actionLogText = "I attacked the enemy. The enemy is still alive.";
                }
                else if (attackResult == "Corpse")
                {
                    actionLogText = "I tried to attack it, but it was already dead.";
                }
                else if (attackResult == "Miss")
                {
                    // まだない
                }
                else Debug.LogWarning($"攻撃結果に対応する処理が無い。: {attackResult}");

                // 攻撃のアニメーションの再生終了を待つ。
                await UniTask.WaitForSeconds(2.0f, cancellationToken: token);
            }
            else
            {
                actionLogText = "There are no enemies around to attack.";
            }

            // 攻撃の結果を行動ログに追加。
            if (TryGetComponent(out ActionLog log)) log.Add(actionLogText);
        }

        string ApplyDamage(Character target, string weapon, int damage)
        {
            if (target == null) return "Miss";

            // Defeat(撃破した)、Hit(当たったが生存)、Corpse(既に死んでいる)、Miss(当たらなかった)
            return target.Damage(_adventurer.ID, weapon, damage, _adventurer.Coords);
        }
    }
}
