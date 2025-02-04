using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ItemData
{
    public enum Usage { None, Throw, Eat }

    public abstract class Item
    {
        string _id = string.Empty;

        public Item(string japaneseName, string englishName, Usage usage)
        {
            Name = new BilingualString(japaneseName, englishName);
            Usage = usage;
        }

        public string ID
        {
            get
            {
                if (_id == string.Empty) _id = GetType().Name;
                return _id;
            }
        }

        public Usage Usage { get; }
        public BilingualString Name { get; }
    }
}