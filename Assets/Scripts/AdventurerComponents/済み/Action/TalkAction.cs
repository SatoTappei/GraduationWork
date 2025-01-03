using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class TalkAction : SurroundingAction
    {
        [SerializeField] ParticleSystem _particle;

        Adventurer _adventurer;
        Animator _animator;
        ActionLog _actionLog;
        LineDisplayer _line;
        TalkThemeSelector _talkTheme;
        TalkPartnerRecord _partner;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _animator = GetComponentInChildren<Animator>();
            _actionLog = GetComponent<ActionLog>();
            _line = GetComponent<LineDisplayer>();
            _talkTheme = GetComponent<TalkThemeSelector>();
            _partner = GetComponent<TalkPartnerRecord>();
        }

        public async UniTask PlayAsync(CancellationToken token)
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;
            const float PlayTime = 3.28f;

            // 周囲に会話可能な冒険者がいない場合。
            if (!TryGetTarget<Adventurer>(out Actor target))
            {
                _actionLog.Add("I tried to talk with other adventurers, but there was no one around.");
                return;
            }

            // 会話する前に目標に向く。
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Talk");
            _line.ShowLine(RequestLineType.Greeting);
            _particle.Play();

            // 目標を向いている間に会話対象が消える可能性があるので事前にチェックする必要がある。
            // 情報の中から会話内容として選んだものを相手に伝え、会話相手を記憶。
            Adventurer targetAdventurer = target as Adventurer;
            if (targetAdventurer != null)
            {
                targetAdventurer.Talk(_talkTheme.Selected, "Adventurer", _adventurer.Coords);
                _partner.Record(targetAdventurer);
            }

            // 会話できたかどうか、結果を行動ログに追加。
            if (targetAdventurer == null)
            {
                _actionLog.Add("I tried to talk with other adventurers, but there was no one around.");
            }
            else
            {
                _actionLog.Add("I talked to the adventurers around me about what I knew.");
            }

            // 会話の演出が終わるまで待つ。
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);
        }
    }
}
