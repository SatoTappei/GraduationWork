using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Barrel : DungeonEntity
    {
        void Start()
        {
            DungeonManager.Find().AddAvoidCell(Coords);
        }

        public string Scavenge()
        {
            return string.Empty;
        }
    }
}