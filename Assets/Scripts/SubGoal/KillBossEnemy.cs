using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class KillBossEnemy : SubGoal
    {
        public KillBossEnemy(Adventurer owner) : base(owner) { }
        public KillBossEnemy(Adventurer owner, SubGoal next) : base(owner, next) { }

        public override bool IsClear()
        {
            throw new System.NotImplementedException();
        }
    }
}