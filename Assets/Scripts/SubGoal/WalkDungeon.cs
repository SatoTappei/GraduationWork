using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WalkDungeon : SubGoal
    {
        public WalkDungeon(Adventurer owner) : base(owner) { }
        public WalkDungeon(Adventurer owner, SubGoal next) : base(owner, next) { }

        public override bool IsClear()
        {
            throw new System.NotImplementedException();
        }
    }
}
