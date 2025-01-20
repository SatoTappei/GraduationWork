using Cysharp.Threading.Tasks;
using Game.ItemThrower;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class ThrowAction : SurroundingAction
    {
        [SerializeField] Grenade _grenade;

        Adventurer _adventurer;
        Animator _animator;
        ItemInventory _item;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _animator = GetComponentInChildren<Animator>();
            _item = GetComponent<ItemInventory>();
        }

        public async UniTask<ActionResult> PlayAsync(CancellationToken token)
        {
            // �V���A���C�Y���Ă��ǂ��B
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            // ���͂ɓG�������͖`���҂�����ꍇ�͌����ē�����B�G��D�悷��B
            if (TryGetTarget<Enemy>(out Actor target) || TryGetTarget<Adventurer>(out target)) { }

            // �ڕW������ꍇ�A����O�ɖڕW�Ɍ����B
            Vector2Int targetDirection;
            if (target != null)
            {
                Cell targetCell = DungeonManager.GetCell(target.Coords);
                Vector3 targetPosition = targetCell.Position;
                await RotateAsync(RotateSpeed, targetPosition, token);

                targetDirection = targetCell.Coords - _adventurer.Coords;
            }
            else
            {
                targetDirection = _adventurer.Direction;
            }

            _animator.Play("Throw");

            // �A�j���[�V�����A�������낷�^�C�~���O�܂ő҂B
            await UniTask.WaitForSeconds(0.5f);

            // �ێ����Ă��铊������A�C�e���̂����A�擪�ɂ�����̂�1�I�ԁB
            ItemData.Item throwItem = null;
            foreach (IReadOnlyList<ItemData.Item> item in _item.Get().Values)
            {
                if (item[0].Usage == ItemData.Usage.Throw)
                {
                    throwItem = item[0];
                    break;
                }
            }

            if (throwItem == null)
            {
                Debug.LogWarning("�A�C�e���𓊂��悤�Ƃ������A�����邱�Ƃ��o����A�C�e���������B");
            }
            else
            {
                _item.Remove(throwItem.Name.Japanese);
            }

            // �A�C�e���𐶐����A������B
            if (throwItem != null && throwItem.Name.Japanese == "��֒e")
            {
                Vector3 position = transform.position + Vector3.up * 0.1f; // �����͋��̈ʒu�ӂ�B 
                Grenade grenade = Instantiate(_grenade, position, Quaternion.identity);
                grenade.Throw(_adventurer.Coords, _adventurer.Direction);
            }
            else
            {
                Debug.LogWarning($"�Ή�����A�C�e���̐������o���Ȃ��B�X�y���~�X�H{throwItem.Name.Japanese}");
            }

            // �A�j���[�V�����Ȃǂ̉��o��҂B
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            return new ActionResult(
                "Throw",
                "Success",
                "I threw the item. And it was lost.",
                _adventurer.Coords,
                targetDirection
            );
        }
    }
}