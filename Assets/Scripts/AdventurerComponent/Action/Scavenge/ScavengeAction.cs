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

            // �s���J�n�̃^�C�~���O�Ŏ��S���Ă����ꍇ�B
            if (_adventurer.Status.CurrentHp <= 0)
            {
                return new ActionResult(
                    "Scavenge",
                    "Failure",
                    $"Died.",
                    _adventurer.Coords,
                    _adventurer.Direction
                );
            }

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
                        _adventurer.Sheet.DisplayID
                    );
                }
                else if(foundItem != null)
                {
                    GameLog.Add(
                        "�V�X�e��",
                        $"{foundItem.Name.Japanese}�����B",
                        LogColor.White,
                        _adventurer.Sheet.DisplayID
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
                    _adventurer.Sheet.DisplayID
                );
            }
            else
            {
                Debug.LogWarning($"���������ʂ̒l�����������B�X�y���~�X�H{result}");
            }

            // ��ɓ��ꂽ�A�C�e���ɂ���đ䎌���ς��B
            if (foundItem == null)
            {
                _line.Show(RequestLineType.GetItemFailure);
            }
            else if (foundItem.Name.English == "Artifact")
            {
                _line.Show(RequestLineType.GetArtifactSuccess);
            }
            else
            {
                _line.Show(RequestLineType.GetItemSuccess);
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

            // ����̃A�j���[�V�����̍Đ��I����҂B
            // ���钆�Ɏ��S�����ꍇ�͒��f����邪�A�擾�͂����B
            for (float i = 0; i <= PlayTime; i += Time.deltaTime)
            {
                if (_adventurer.Status.CurrentHp <= 0) break;

                await UniTask.Yield(cancellationToken: token);
            }

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
            if (foundItem == null) { }
            else if (foundItem.Name.Japanese == "�N���b�J�[")
            {
                VantanConnect.SendEvent(new EventData(EventDefine.ActorEffect));
            }
            else if (foundItem.Name.Japanese == "��ꂽ�")
            {
                // 
            }
            else if (foundItem.Name.Japanese == "�K�т���")
            {
                VantanConnect.SendEvent(new EventData(EventDefine.PickupItem));
            }
            else if (foundItem.Name.Japanese == "�؂ꂽ�d��")
            {
                VantanConnect.SendEvent(new EventData(EventDefine.DarkRoom));
            }
            else if (foundItem.Name.Japanese == "�w�����b�g")
            {
                VantanConnect.SendEvent(new EventData(EventDefine.SummonEnemy));
            }
            else if (foundItem.Name.Japanese == "���A�[�e�B�t�@�N�g")
            {
                EventData data = new EventData(EventDefine.GetArtifact);
                data.DataPack("UserId", _adventurer.Sheet.UserId);
                VantanConnect.SendEvent(data);
            }

            // �A�C�e������肵�����Ƃ��G�s�\�[�h�Ƃ��đ��M�B
            if (foundItem != null)
            {
                GameEpisode episode = new GameEpisode(
                    EpisodeCode.VCMainItem,
                    _adventurer.Sheet.UserId
                );
                episode.SetEpisode("�A�C�e������肵��");
                episode.DataPack("���肵���A�C�e��", foundItem.Name.Japanese);
                VantanConnect.SendEpisode(episode);
            }

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
