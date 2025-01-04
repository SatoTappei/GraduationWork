using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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

        public async UniTask<string> PlayAsync<TTarget>(CancellationToken token) where TTarget : IDamageable
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            // 周囲に攻撃可能な対象が居ない場合。
            if (!TryGetTarget<TTarget>(out Actor target))
            {
                return "There are no enemies around to attack.";
            }

            // 攻撃する前に目標に向く。
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Attack");
            _line.ShowLine(RequestLineType.Attack);

            // ダメージを与える。
            string result = string.Empty;
            if (target != null && target is Character targetCharacter)
            {
                // Defeat(撃破した)、Hit(当たったが生存)、Corpse(既に死んでいる)、Miss(当たらなかった)
                result = targetCharacter.Damage(_adventurer.Status.TotalAttack, _adventurer.Coords);
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
                    $"{_adventurer.AdventurerSheet.DisplayName}が敵を倒した。",
                    GameLogColor.White
                );

                _adventurer.Status.DefeatCount++;
            }

            // 攻撃結果を返す。
            if (result == "Defeat")
            {
                return "I attacked the enemy. I defeated the enemy.";
            }
            else if (result == "Hit")
            {
                return "I attacked the enemy. The enemy is still alive.";
            }
            else if (result == "Corpse")
            {
                return "I tried to attack it, but it was already dead.";
            }
            else if (result == "Miss")
            {
                // まだない
                return "I tried to attack it, but it was miss.";
            }
            else
            {
                Debug.LogWarning($"攻撃結果に対応する処理が無い。スペルミス？: {result}");
                return string.Empty;
            }
        }
    }
}