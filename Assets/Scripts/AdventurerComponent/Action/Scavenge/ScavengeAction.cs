using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class ScavengeAction : SurroundingAction
    {
        Adventurer _adventurer;
        Animator _animator;
        ItemInventory _inventory;
        LineDisplayer _line;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _animator = GetComponentInChildren<Animator>();
            _inventory = GetComponent<ItemInventory>();
            _line = GetComponent<LineDisplayer>();
        }

        public async UniTask<ActionResult> PlayAsync(CancellationToken token)
        {
            // �V���A���C�Y���Ă��ǂ��B
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            // ����ۂ͕󔠂�D�悷��B
            if (TryGetTarget<Treasure>(out Actor target) || TryGetTarget<IScavengeable>(out target)) { }

            // ���͂ɋ�����̂������ꍇ�B
            if (target == null)
            {
                return new ActionResult(
                    "There are no areas around that can be scavenged.",
                    _adventurer.Coords,
                    _adventurer.Direction
                );
            }

            // ����O�ɖڕW�Ɍ����B
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Scav");

            // �A�C�e��������A��������̃A�C�e�����l�������ꍇ�̓C���x���g���ɒǉ��B
            Item foundItem = null;
            if (target != null && target is IScavengeable targetEntity)
            {
                foundItem = targetEntity.Scavenge();

                if (foundItem != null) _inventory.Add(foundItem);
            }

            // ����������肵���ꍇ�A���O�ɕ\���B
            if (foundItem == null)
            {
                // �܂��Ȃ�
            }
            else if (foundItem.Name.English == "Treasure")
            {
                GameLog.Add(
                    "�V�X�e��", 
                    $"{_adventurer.AdventurerSheet.DisplayName}���󕨂����B", 
                    GameLogColor.White
                );
            }
            else
            {
                GameLog.Add(
                    "�V�X�e��", 
                    $"{_adventurer.AdventurerSheet.DisplayName}���A�C�e�������B", 
                    GameLogColor.White
                );
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

            // �󔠂��l�������ꍇ�A�󔠃J�E���g�𑝂₷�B
            if (foundItem != null && foundItem.Name.English == "Treasure")
            {
                _adventurer.Status.TreasureCount++;
            }

            // �A�j���[�V�����Ȃǂ̉��o��҂B
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            // ���������ʂ��Ƃ̍s�����O�ɒǉ����镶�́B
            string actionLog;
            if (foundItem == null)
            {
                actionLog = "I scavenged the surrounding boxes. There was nothing in them.";
            }
            else if (foundItem.Name.English == "Treasure")
            {
                actionLog = "I scavenged the surrounding treasure chests. I got the treasure.";
            }
            else
            {
                actionLog = $"I scavenged the surrounding boxes. I got the {foundItem}.";
            }

            return new ActionResult(
                actionLog,
                _adventurer.Coords,
                _adventurer.Direction
            );
        } 
    }
}
