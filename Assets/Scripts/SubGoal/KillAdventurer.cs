using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class KillAdventurer : SubGoal
    {
        public KillAdventurer(Adventurer owner) : base(owner) { }
        public KillAdventurer(Adventurer owner, SubGoal next) : base(owner, next) { }

        public override bool IsClear()
        {
            throw new System.NotImplementedException();
        }
    }
}
