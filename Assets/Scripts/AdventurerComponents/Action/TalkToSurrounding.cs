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
            // �V���A���C�Y���Ă��ǂ��B
            const float RotateSpeed = 4.0f;
            const float PlayTime = 3.28f;

            token.ThrowIfCancellationRequested();

            // ��b����Ώۂ����邩�ǂ����ōs�����O�ɒǉ�������e���قȂ�B
            string actionLogText = string.Empty;

            // ���͂ɖ`���҂�����ꍇ�͉�b�B
            if (TryGetTarget<Adventurer>(out Actor target))
            {
                // ��b����O�ɖڕW�Ɍ����B
                Vector3 targetPosition = _dungeonManager.GetCell(target.Coords).Position;
                await RotateAsync(RotateSpeed, targetPosition, token);

                _animator.Play("Talk");

                // �b��������ۂ̑䎌��\���B��b���e�Ƃ͕ʁB
                if (TryGetComponent(out LineApply line)) line.ShowLine(RequestLineType.Greeting);
                
                // ��b���̃G�t�F�N�g���Đ��B
                if (TryGetComponent(out TalkEffect effect)) effect.Play();
                
                ApplyTalk(target as Adventurer);
                
                actionLogText = "I talked to the adventurers around me about what I knew.";

                // ��b�̉��o���I���܂ő҂B
                await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);
            }
            else
            {
                actionLogText = "I tried to talk with other adventurers, but there was no one around.";
            }

            // �U���̌��ʂ��s�����O�ɒǉ��B
            if (TryGetComponent(out ActionLog log)) log.Add(actionLogText);
        }

        void ApplyTalk(Adventurer target)
        {
            if (target == null) return;
            if (!TryGetComponent(out InformationStock information)) return;

            // ���̒������b���e�Ƃ��đI�񂾂��̂𑊎�ɓ`����B
            target.Talk(information.TalkTheme, "Adventurer", _adventurer.Coords);
        }
    }
}
