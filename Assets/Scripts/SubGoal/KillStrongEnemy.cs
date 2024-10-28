using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class KillStrongEnemy : SubGoal
    {
        public KillStrongEnemy(Adventurer owner) : base(owner) { }
        public KillStrongEnemy(Adventurer owner, SubGoal next) : base(owner, next) { }

        public override bool IsClear()
        {
            throw new System.NotImplementedException();
        }
    }
}
