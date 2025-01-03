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
            // �V���A���C�Y���Ă��ǂ��B
            const float RotateSpeed = 4.0f;
            const float PlayTime = 3.28f;

            // ���͂ɉ�b�\�Ȗ`���҂����Ȃ��ꍇ�B
            if (!TryGetTarget<Adventurer>(out Actor target))
            {
                _actionLog.Add("I tried to talk with other adventurers, but there was no one around.");
                return;
            }

            // ��b����O�ɖڕW�Ɍ����B
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Talk");
            _line.ShowLine(RequestLineType.Greeting);
            _particle.Play();

            // �ڕW�������Ă���Ԃɉ�b�Ώۂ�������\��������̂Ŏ��O�Ƀ`�F�b�N����K�v������B
            // ���̒������b���e�Ƃ��đI�񂾂��̂𑊎�ɓ`���A��b������L���B
            Adventurer targetAdventurer = target as Adventurer;
            if (targetAdventurer != null)
            {
                targetAdventurer.Talk(_talkTheme.Selected, "Adventurer", _adventurer.Coords);
                _partner.Record(targetAdventurer);
            }

            // ��b�ł������ǂ����A���ʂ��s�����O�ɒǉ��B
            if (targetAdventurer == null)
            {
                _actionLog.Add("I tried to talk with other adventurers, but there was no one around.");
            }
            else
            {
                _actionLog.Add("I talked to the adventurers around me about what I knew.");
            }

            // ��b�̉��o���I���܂ő҂B
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);
        }
    }
}
