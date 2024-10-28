using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class KillWeakEnemy : SubGoal
    {
        public KillWeakEnemy(Adventurer owner) : base(owner) { }
        public KillWeakEnemy(Adventurer owner, SubGoal next) : base(owner, next) { }

        public override bool IsClear()
        {
            throw new System.NotImplementedException();
        }
    }
}
