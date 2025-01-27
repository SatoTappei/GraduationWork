using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Game.ItemData;
using VTNConnect;

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

            // ����ۂ̗D��x�� ��->��->���̑� �̏��B
            if (TryGetTarget<TreasureChestKey>(out Actor target) || 
                TryGetTarget<Treasure>(out target) || 
                TryGetTarget<IScavengeable>(out target)) { }

            // ���͂ɋ�����̂������ꍇ�B
            if (target == null)
            {
                return new ActionResult(
                    "Scavenge",
                    "Failure",
                    "There are no areas around that can be scavenged.",
                    _adventurer.Coords,
                    _adventurer.Direction
                );
            }

            // ����O�ɖڕW�Ɍ����B
            Cell targetCell = DungeonManager.GetCell(target.Coords);
            Vector3 targetPosition = targetCell.Position;
            Vector2Int targetDirection = targetCell.Coords - _adventurer.Coords;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Scav");

            // �A�C�e��������A��������̃A�C�e�����l�������ꍇ�̓C���x���g���ɒǉ��B
            Item foundItem = null;
            string result = string.Empty;
            if (target != null && target is IScavengeable targetEntity)
            {
                result = targetEntity.Scavenge(_adventurer, out foundItem);

                if (foundItem != null) _inventory.Add(foundItem);
            }

            // ���������ʂɂ���ă��O�ɒǉ�������e���ς��B
            if (result == "Get")
            {
                if (foundItem != null && foundItem.Name.English == "Artifact")
                {
                    GameLog.Add(
                        "�V�X�e��",
                        $"{foundItem.Name.Japanese}�����I",
                        LogColor.Yellow,
                        _adventurer.AdventurerSheet.Number
                    );
                }
                else if(foundItem != null)
                {
                    GameLog.Add(
                        "�V�X�e��",
                        $"{foundItem.Name.Japanese}�����B",
                        LogColor.White,
                        _adventurer.AdventurerSheet.Number
                    );
                }
            }
            else if (result == "Empty")
            {
                //
            }
            else if (result == "Lock")
            {
                GameLog.Add(
                    "�V�X�e��",
                    $"�󔠂̌��������Ă��Ȃ��B",
                    LogColor.White,
                    _adventurer.AdventurerSheet.Number
                );
            }
            else
            {
                Debug.LogWarning($"���������ʂ̒l�����������B�X�y���~�X�H{result}");
            }

            // ��ɓ��ꂽ�A�C�e���ɂ���đ䎌���ς��B
            if (foundItem == null)
            {
                _line.ShowLine(RequestLineType.GetItemFailure);
            }
            else if (foundItem.Name.English == "Artifact")
            {
                _line.ShowLine(RequestLineType.GetArtifactSuccess);
            }
            else
            {
                _line.ShowLine(RequestLineType.GetItemSuccess);
            }

            // ��ɓ��ꂽ�A�C�e���ɑΉ������J�E���g�𑝂₷�B
            if (foundItem != null && foundItem.Name.English == "Artifact")
            {
                _adventurer.Status.ArtifactCount++;
            }
            if (foundItem != null && foundItem.Name.English == "Treasure")
            {
                _adventurer.Status.TreasureCount++;
            }

            // �A�j���[�V�����Ȃǂ̉��o��҂B
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            // ���������ʂ��Ƃ̍s�����O�ɒǉ����镶�́B
            string actionLog;
            if (result == "Get")
            {
                if (foundItem != null && foundItem.Name.English == "Artifact")
                {
                    actionLog = "I scavenged the surrounding altar. I got the legendary treasure.";
                }
                else if (foundItem != null && foundItem.Name.English == "Treasure")
                {
                    actionLog = "I scavenged the surrounding treasure chests. I got the treasure.";
                }
                else if (foundItem != null)
                {
                    actionLog = $"I scavenged the surrounding boxes. I got the {foundItem.Name.English}.";
                }
                else
                {
                    actionLog = "I scavenged the surrounding boxes. There was nothing in them.";
                }
            }
            else if (result == "Empty")
            {
                actionLog = "I scavenged the surrounding boxes. There was nothing in them.";
            }
            else if (result == "Lock")
            {
                actionLog = "I did not have the keys to open the surrounding treasure chests.";
            }
            else
            {
                Debug.LogWarning($"���������ʂ̒l�����������B�X�y���~�X�H{result}");
                actionLog = "I scavenged the surrounding boxes. There was nothing in them.";
            }

            // ���肵���A�C�e���ɑΉ������C�x���g������ꍇ�͑��M�B
            EventData eventData = null;
            if (foundItem == null) { }
            else if (foundItem.Name.Japanese == "�N���b�J�[")
            {
                eventData = new EventData(EventDefine.ActorEffect);
            }
            else if (foundItem.Name.Japanese == "��ꂽ�")
            {
                eventData = new EventData(EventDefine.ReviveGimmick);
            }
            else if (foundItem.Name.Japanese == "�K�т���")
            {
                eventData = new EventData(EventDefine.PickupItem);
            }
            else if (foundItem.Name.Japanese == "�؂ꂽ�d��")
            {
                eventData = new EventData(EventDefine.DarkRoom);
            }
            else if (foundItem.Name.Japanese == "�w�����b�g")
            {
                eventData = new EventData(EventDefine.SummonEnemy);
            }

            if (eventData != null) VantanConnect.SendEvent(eventData);

            // ��������A�C�e�������o����ΐ����A�o���Ȃ���Ύ��s�B
            return new ActionResult(
                "Scavenge",
                foundItem != null ? "Success" : "Failure",
                actionLog,
                _adventurer.Coords,
                targetDirection,
                _adventurer.Coords + targetDirection
            );
        } 
    }
}
