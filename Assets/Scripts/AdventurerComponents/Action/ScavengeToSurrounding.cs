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
        ItemInventory _inventory;
        LineApply _line;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _animator = GetComponentInChildren<Animator>();
            _line = GetComponent<LineApply>();
        }

        public async UniTask ScavengeAsync(CancellationToken token)
        {
            // �V���A���C�Y���Ă��ǂ��B
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            // ����ۂ͕󔠂�D�悷��B
            if (TryGetTarget<Treasure>(out Actor target) || TryGetTarget<IScavengeable>(out target)) { }

            // ���͂ɋ�����̂������ꍇ�B
            if (target == null)
            {
                _actionLog.Add("There are no areas around that can be scavenged.");
                return;
            }

            // ����O�ɖڕW�Ɍ����B
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Scav");

            // �A�C�e��������B
            Item foundItem;
            if (target is IScavengeable targetEntity)
            {
                foundItem = targetEntity.Scavenge();
            }

            _inventory.Add(foundItem);

            // ����������肵���ꍇ�A���O�ɕ\���B
            if (foundItem == null)
            {
                // �܂��Ȃ�
            }
            else if (foundItem.Name.English == "Treasure")
            {
                GameLog.Add("�V�X�e��", $"{_blackboard.DisplayName}���󕨂����B", GameLogColor.White);
            }
            else
            {
                GameLog.Add("�V�X�e��", $"{_blackboard.DisplayName}���A�C�e�������B", GameLogColor.White);
            }

            // ���������ʂ��s�����O�ɒǉ��B
            if (foundItem == null)
            {
                _actionLog.Add("I scavenged the surrounding boxes. There was nothing in them.");
            }
            else if (foundItem.Name.English == "Treasure")
            {
                _actionLog.Add("I scavenged the surrounding treasure chests. I got the treasure.");
            }
            else
            {
                _actionLog.Add($"I scavenged the surrounding boxes. I got the {foundItem}.");
            }

            // ���������ʂɉ������䎌�B
            if (foundItem == null)
            {
                _line.ShowLine(RequestLineType.GetItemFailure);
            }
            else if (foundItem.Name.English == "Treasure")
            {
                _line.ShowLine(RequestLineType.GetTreasureSuccess);
            }
            else
            {
                _line.ShowLine(RequestLineType.GetItemSuccess);
            }

            // �󔠂��l�������ꍇ

            // �A�j���[�V�����Ȃǂ̉��o��҂B
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            {
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
                if (gameLogText != string.Empty)
                {
                    GameLog.Add("�V�X�e��", gameLogText, GameLogColor.White);
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
