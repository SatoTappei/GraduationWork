using System.Collections;
using System.Collections.Generic;

namespace Game
{
    public class GetRequestedItem : SubGoal
    {
        public static readonly BilingualString StaticText;

        static GetRequestedItem()
        {
            string j = "依頼されたアイテムを手に入れる。";
            string e = "Scavenge containers and barrels to obtain items.";
            StaticText = new BilingualString(j, e);
        }

        public GetRequestedItem(Adventurer owner) : base(owner) { }

        public override BilingualString Text => StaticText;

        public override bool IsCompleted()
        {
            foreach (InventoryItem item in Owner.ItemInventory.GetAllInventoryItem())
            {
                if (item.Name == "依頼されたアイテム") return true;
            }

            return false;
        }
    }
}