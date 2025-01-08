using UnityEngine;

namespace Game
{
    public class FindItemGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;
        ItemInventory _itemInventory;

        void Awake()
        {
            _description = new BilingualString(
                "依頼されたアイテムを手に入れる。", 
                "Scavenge containers and barrels to obtain items."
            );
            _adventurer = GetComponent<Adventurer>();
            _itemInventory = GetComponent<ItemInventory>();
        }

        public override BilingualString Description => _description;

        public override bool IsCompleted()
        {
            // アイテムは自由に設定しても大丈夫。
            foreach (ItemInventory.Entry item in _itemInventory.GetEntries())
            {
                if (item.Name == "荷物") return true;
            }

            return false;
        }

        public override bool IsRetire()
        {
            return _adventurer.Status.ElapsedTurn > 100;
        }
    }
}