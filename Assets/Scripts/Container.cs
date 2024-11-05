using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Container : DungeonEntity, IScavengeable
    {
        void Start()
        {
            DungeonManager.Find().AddAvoidCell(Coords);
        }

        public Item Scavenge()
        {
            return new Item("ˆË—Š‚³‚ê‚½ƒAƒCƒeƒ€", "RequestedItem");
        }
    }
}
