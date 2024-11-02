using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public GetRequestedItem(Adventurer owner) : base(owner) { }

        public override BilingualString Text => StaticText;

        public override bool IsCompleted()
        {
            return Owner.Item.Contains("�˗����ꂽ�A�C�e��");
        }
    }
}