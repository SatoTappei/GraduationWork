using System.Collections;
using System.Collections.Generic;

namespace Game
{
    public class GetRequestedItem : SubGoal
    {
        public static readonly BilingualString StaticText;

        static GetRequestedItem()
        {
            string j = "�˗����ꂽ�A�C�e������ɓ����B";
            string e = "Scavenge containers and barrels to obtain items.";
            StaticText = new BilingualString(j, e);
        }

        public GetRequestedItem(IReadOnlyAdventurerContext context) : base(context) { }

        public override BilingualString Text => StaticText;

        public override bool IsCompleted()
        {
            foreach (ItemInventory.Entry item in Context.Item)
            {
                if (item.Name == "�˗����ꂽ�A�C�e��") return true;
            }

            return false;
        }
    }
}