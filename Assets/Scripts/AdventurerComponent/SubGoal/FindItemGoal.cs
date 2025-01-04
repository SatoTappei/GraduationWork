using UnityEngine;

namespace Game
{
    public class FindItemGoal : SubGoal
    {
        BilingualString _description;
        ItemInventory _itemInventory;

        void Awake()
        {
            _description = new BilingualString(
                "�˗����ꂽ�A�C�e������ɓ����B", 
                "Scavenge containers and barrels to obtain items."
            );
            _itemInventory = GetComponent<ItemInventory>();
        }

        public override BilingualString Description => _description;

        public override bool IsCompleted()
        {
            // �A�C�e���͎��R�ɐݒ肵�Ă����v�B
            foreach (ItemInventory.Entry item in _itemInventory.GetEntries())
            {
                if (item.Name == "�ו�") return true;
            }

            return false;
        }
    }
}