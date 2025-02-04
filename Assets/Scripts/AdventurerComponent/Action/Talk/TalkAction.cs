using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VTNConnect;

namespace Game
{
    public class TalkAction : SurroundingAction
    {
        [SerializeField] ParticleSystem _particle;

        Adventurer _adventurer;
        Animator _animator;
        LineDisplayer _line;
        TalkThemeSelector _talkTheme;

        // 話しかけた相手を記録する。
        List<string> _history;

        public IReadOnlyList<string> History => _history;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _animator = GetComponentInChildren<Animator>();
            _line = GetComponent<LineDisplayer>();
            _talkTheme = GetComponent<TalkThemeSelector>();

            _history = new List<string>();
        }

        public async UniTask<ActionResult> PlayAsync(CancellationToken token)
        {
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;
            const float PlayTime = 3.28f;

            // 行動開始のタイミングで死亡していた場合。
            if (_adventurer.Status.CurrentHp <= 0)
            {
                return new ActionResult(
                    "Talk",
                    "Failure",
                    $"Died.",
                    _adventurer.Coords,
                    _adventurer.Direction
                );
            }

            // 周囲に会話可能な冒険者がいない場合。
            if (!TryGetTarget<Adventurer>(out Actor target))
            {
                return new ActionResult(
                    "Talk",
                    "Failure",
                    "I tried to talk with other adventurers, but there was no one around.",
                    _adventurer.Coords,
                    _adventurer.Direction
                );
            }

            // 会話する前に目標に向く。
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Talk");
            _line.ShowLine(RequestLineType.Greeting);
            _particle.Play();

            // 目標を向いている間に会話対象が消える可能性があるので事前にチェックする必要がある。
            // 情報の中から会話内容として選んだものを相手に伝える。
            bool isTalked = true;
            if (target != null && target.TryGetComponent(out TalkReceiver talk))
            {
                talk.Talk(_talkTheme.Selected, "Adventurer", _adventurer.Coords);
            }
            else
            {
                isTalked = false;
            }

            Adventurer targetAdventurer = null;
            if (target != null) targetAdventurer = target.GetComponent<Adventurer>();

            // 1度も会話をしたことのない相手の場合、エピソードを送信する。
            if (targetAdventurer != null)
            {
                string targetName = targetAdventurer.Sheet.FullName;
                if (!_history.Contains(targetName))
                {
                    GameEpisode episode = new GameEpisode(
                        EpisodeCode.VCMainTalk,
                        _adventurer.Sheet.UserId
                    );
                    episode.SetEpisode("冒険者と会話した");
                    episode.DataPack("会話した相手", targetName);
                    VantanConnect.SendEpisode(episode);
                }
            }

            // 会話相手を記録。
            if (targetAdventurer != null)
            {
                _history.Add(targetAdventurer.Sheet.FullName);
            }
            else if (target != null)
            {
                _history.Add(target.ID);
            }

            // 会話の演出が終わるまで待つ。会話中に死亡した場合は中断される。
            for (float i = 0; i <= PlayTime; i += Time.deltaTime)
            {
                if (_adventurer.Status.CurrentHp <= 0) break;

                await UniTask.Yield(cancellationToken: token);
            }

            // 会話できたかどうか、結果を返す。
            if (isTalked)
            {
                return new ActionResult(
                    "Talk",
                    "Success",
                    "I talked to the adventurers around me about what I knew.",
                    _adventurer.Coords,
                    _adventurer.Direction
                );
            }
            else
            {
                return new ActionResult(
                    "Talk",
                    "Failure",
                    "I tried to talk with other adventurers, but there was no one around.", 
                    _adventurer.Coords, 
                    _adventurer.Direction
                );
            }
        }
    }
}
