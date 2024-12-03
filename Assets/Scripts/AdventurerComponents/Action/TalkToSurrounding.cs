using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class TalkToSurrounding : SurroundingAction
    {
        DungeonManager _dungeonManager;
        Adventurer _adventurer;
        Animator _animator;

        void Awake()
        {
            DungeonManager.TryFind(out _dungeonManager);
            _adventurer = GetComponent<Adventurer>();
            _animator = GetComponentInChildren<Animator>();
        }

        public async UniTask TalkAsync(CancellationToken token)
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;
            const float PlayTime = 3.28f;

            token.ThrowIfCancellationRequested();

            // 会話する対象がいるかどうかで行動ログに追加する内容が異なる。
            string actionLogText = string.Empty;

            // 周囲に冒険者がいる場合は会話。
            if (TryGetTarget<Adventurer>(out Actor target))
            {
                // 会話する前に目標に向く。
                Vector3 targetPosition = _dungeonManager.GetCell(target.Coords).Position;
                await RotateAsync(RotateSpeed, targetPosition, token);

                _animator.Play("Talk");

                // 話しかける際の台詞を表示。会話内容とは別。
                if (TryGetComponent(out LineApply line)) line.ShowLine(RequestLineType.Greeting);
                
                // 会話中のエフェクトを再生。
                if (TryGetComponent(out TalkEffect effect)) effect.Play();
                
                ApplyTalk(target as Adventurer);
                
                actionLogText = "I talked to the adventurers around me about what I knew.";

                // 会話の演出が終わるまで待つ。
                await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);
            }
            else
            {
                actionLogText = "I tried to talk with other adventurers, but there was no one around.";
            }

            // 攻撃の結果を行動ログに追加。
            if (TryGetComponent(out ActionLog log)) log.Add(actionLogText);
        }

        void ApplyTalk(Adventurer target)
        {
            if (target == null) return;
            if (!TryGetComponent(out InformationStock information)) return;

            // 情報の中から会話内容として選んだものを相手に伝える。
            target.Talk(information.TalkTheme, "Adventurer", _adventurer.Coords);
        }
    }
}
