using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ItemEntry
    {
        public ItemEntry(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public string Name;
        public int Count;
    }
}
