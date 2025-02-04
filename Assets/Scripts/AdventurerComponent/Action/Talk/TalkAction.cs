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

        // �b��������������L�^����B
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
            // �V���A���C�Y���Ă��ǂ��B
            const float RotateSpeed = 4.0f;
            const float PlayTime = 3.28f;

            // �s���J�n�̃^�C�~���O�Ŏ��S���Ă����ꍇ�B
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

            // ���͂ɉ�b�\�Ȗ`���҂����Ȃ��ꍇ�B
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

            // ��b����O�ɖڕW�Ɍ����B
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Talk");
            _line.ShowLine(RequestLineType.Greeting);
            _particle.Play();

            // �ڕW�������Ă���Ԃɉ�b�Ώۂ�������\��������̂Ŏ��O�Ƀ`�F�b�N����K�v������B
            // ���̒������b���e�Ƃ��đI�񂾂��̂𑊎�ɓ`����B
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

            // 1�x����b���������Ƃ̂Ȃ�����̏ꍇ�A�G�s�\�[�h�𑗐M����B
            if (targetAdventurer != null)
            {
                string targetName = targetAdventurer.Sheet.FullName;
                if (!_history.Contains(targetName))
                {
                    GameEpisode episode = new GameEpisode(
                        EpisodeCode.VCMainTalk,
                        _adventurer.Sheet.UserId
                    );
                    episode.SetEpisode("�`���҂Ɖ�b����");
                    episode.DataPack("��b��������", targetName);
                    VantanConnect.SendEpisode(episode);
                }
            }

            // ��b������L�^�B
            if (targetAdventurer != null)
            {
                _history.Add(targetAdventurer.Sheet.FullName);
            }
            else if (target != null)
            {
                _history.Add(target.ID);
            }

            // ��b�̉��o���I���܂ő҂B��b���Ɏ��S�����ꍇ�͒��f�����B
            for (float i = 0; i <= PlayTime; i += Time.deltaTime)
            {
                if (_adventurer.Status.CurrentHp <= 0) break;

                await UniTask.Yield(cancellationToken: token);
            }

            // ��b�ł������ǂ����A���ʂ�Ԃ��B
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
