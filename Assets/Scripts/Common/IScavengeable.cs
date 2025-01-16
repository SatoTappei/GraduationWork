using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.ItemData;

namespace Game
{
    public interface IScavengeable
    {
        public Item Scavenge();
    }
}
