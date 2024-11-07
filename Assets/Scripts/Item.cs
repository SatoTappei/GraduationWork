using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Item
    {
        public Item(string japaneseName, string englishName)
        {
            Name = new BilingualString(japaneseName, englishName);
        }

        public Item(BilingualString name)
        {
            Name = name;
        }

        public BilingualString Name { get; }
    }
}