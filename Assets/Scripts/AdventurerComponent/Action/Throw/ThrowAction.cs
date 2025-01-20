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
            // シリアライズしても良い。
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            // 周囲に敵もしくは冒険者がいる場合は向いて投げる。敵を優先する。
            if (TryGetTarget<Enemy>(out Actor target) || TryGetTarget<Adventurer>(out target)) { }

            // 目標がいる場合、漁る前に目標に向く。
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

            // アニメーション、投げおろすタイミングまで待つ。
            await UniTask.WaitForSeconds(0.5f);

            // 保持している投げられるアイテムのうち、先頭にあるものを1つ選ぶ。
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
                Debug.LogWarning("アイテムを投げようとしたが、投げることが出来るアイテムが無い。");
            }
            else
            {
                _item.Remove(throwItem.Name.Japanese);
            }

            // アイテムを生成し、投げる。
            if (throwItem != null && throwItem.Name.Japanese == "手榴弾")
            {
                Vector3 position = transform.position + Vector3.up * 0.1f; // 高さは胸の位置辺り。 
                Grenade grenade = Instantiate(_grenade, position, Quaternion.identity);
                grenade.Throw(_adventurer.Coords, _adventurer.Direction);
            }
            else
            {
                Debug.LogWarning($"対応するアイテムの生成が出来ない。スペルミス？{throwItem.Name.Japanese}");
            }

            // アニメーションなどの演出を待つ。
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