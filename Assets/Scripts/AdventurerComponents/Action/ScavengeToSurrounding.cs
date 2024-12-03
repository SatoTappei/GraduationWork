using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class ScavengeToSurrounding : SurroundingAction
    {
        Blackboard _blackboard;
        Animator _animator;
        DungeonManager _dungeonManager;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _animator = GetComponentInChildren<Animator>();
            DungeonManager.TryFind(out _dungeonManager);
        }

        public async UniTask ScavengeAsync(CancellationToken token)
        {
            // �V���A���C�Y���Ă��ǂ��B
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            token.ThrowIfCancellationRequested();

            // ���������ʂɂ���čs�����O�ɒǉ�������e���قȂ�B
            string actionLogText = string.Empty;

            // ����ۂ͕󔠂�D�悷��B
            if (TryGetTarget<Treasure>(out Actor target) || TryGetTarget<IScavengeable>(out target))
            {
                // ����O�ɖڕW�Ɍ����B
                Vector3 targetPosition = _dungeonManager.GetCell(target.Coords).Position;
                await RotateAsync(RotateSpeed, targetPosition, token);

                _animator.Play("Scav");

                // ���������ʂɉ����ăQ�[���i�s���O�Ƒ䎌�ɒǉ�������e���قȂ�B
                string gameLogText = string.Empty;
                RequestLineType lineType = RequestLineType.None;

                // �A�C�e�����擾�B
                string foundItem = ApplyScavenge(target as IScavengeable);
                if (foundItem == "Treasure")
                {
                    gameLogText = $"{_blackboard.DisplayName}���󕨂����B";
                    actionLogText = "I scavenged the surrounding treasure chests. I got the treasure.";
                    lineType = RequestLineType.GetTreasureSuccess;

                    _blackboard.TreasureCount++;
                }
                else if (foundItem == "Empty")
                {
                    actionLogText = "I scavenged the surrounding boxes. There was nothing in them.";
                    lineType = RequestLineType.GetItemFailure;
                }
                else
                {
                    gameLogText = $"{_blackboard.DisplayName}���A�C�e�������B";
                    actionLogText = $"I scavenged the surrounding boxes. I got the {foundItem}.";
                    lineType = RequestLineType.GetItemSuccess;
                }

                // �\��������e������ꍇ�̓Q�[���i�s���O�ɕ\���B
                if (gameLogText != string.Empty && UiManager.TryFind(out UiManager ui))
                {
                    ui.AddLog(gameLogText);
                }

                // ���������ʂɉ������䎌��\���B
                if (TryGetComponent(out LineApply line)) line.ShowLine(lineType);

                // �A�j���[�V�����Ȃǂ̉��o��҂B
                await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);
            }
            else
            {
                actionLogText = "There are no areas around that can be scavenged.";
            }

            // ���������ʂ��s�����O�ɒǉ��B
            if (TryGetComponent(out ActionLog log)) log.Add(actionLogText);
        }

        string ApplyScavenge(IScavengeable target)
        {
            if (target == null) return "Empty";

            Item result = target.Scavenge();
            if (result != null)
            {
                // �擾�����A�C�e�����C���x���g���ɒǉ��B
                if (TryGetComponent(out ItemInventory inventory)) inventory.Add(result);

                return result.Name.English;
            }
            else
            {
                return "Empty";
            }
        }
    }
}
