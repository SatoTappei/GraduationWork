using System.Collections;
using System.Collections.Generic;

namespace Game
{
    public class GetRequestedItem : SubGoal
    {
        BilingualString _text;
        ItemInventory _itemInventory;

        void Awake()
        {
            _text = new BilingualString("�˗����ꂽ�A�C�e������ɓ����B", "Scavenge containers and barrels to obtain items.");
            _itemInventory = GetComponent<ItemInventory>();
        }

        public override BilingualString Text => _text;

        public override bool IsCompleted()
        {
            foreach (ItemInventory.Entry item in _itemInventory.Entries)
            {
                if (item.Name == "�˗����ꂽ�A�C�e��") return true;
            }

            return false;
        }
    }
}