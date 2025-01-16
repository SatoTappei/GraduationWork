using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ItemData
{
    public enum Usage { None, Throw, Eat }

    public abstract class Item
    {
        public Item(string japaneseName, string englishName, Usage usage)
        {
            Name = new BilingualString(japaneseName, englishName);
            Usage = usage;
        }

        public Usage Usage { get; }
        public BilingualString Name { get; }
    }
}