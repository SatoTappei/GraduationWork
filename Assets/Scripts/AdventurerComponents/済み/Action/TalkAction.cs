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
        LineDisplayer _line;
        TalkThemeSelector _talkTheme;
        TalkPartnerRecord _partner;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _animator = GetComponentInChildren<Animator>();
            _line = GetComponent<LineDisplayer>();
            _talkTheme = GetComponent<TalkThemeSelector>();
            _partner = GetComponent<TalkPartnerRecord>();
        }

        public async UniTask<string> PlayAsync(CancellationToken token)
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;
            const float PlayTime = 3.28f;

            // 周囲に会話可能な冒険者がいない場合。
            if (!TryGetTarget<Adventurer>(out Actor target))
            {
                return "I tried to talk with other adventurers, but there was no one around.";
            }

            // 会話する前に目標に向く。
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Talk");
            _line.ShowLine(RequestLineType.Greeting);
            _particle.Play();

            // 目標を向いている間に会話対象が消える可能性があるので事前にチェックする必要がある。
            // 情報の中から会話内容として選んだものを相手に伝え、会話相手を記憶。
            bool isTalked = true;
            if (target != null && target.TryGetComponent(out TalkReceiver talk))
            {
                talk.Talk(_talkTheme.Selected, "Adventurer", _adventurer.Coords);
                _partner.Record(target as Adventurer);
            }
            else
            {
                isTalked = false;
            }

            // 会話の演出が終わるまで待つ。
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            // 会話できたかどうか、結果を返す。
            if (isTalked)
            {
                return "I talked to the adventurers around me about what I knew.";
            }
            else
            {
                return "I tried to talk with other adventurers, but there was no one around.";
            }
        }
    }
}
