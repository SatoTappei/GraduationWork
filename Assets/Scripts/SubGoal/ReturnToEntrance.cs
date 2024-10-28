using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ReturnToEntrance : SubGoal
    {
        public ReturnToEntrance(Adventurer owner) : base(owner) { }
        public ReturnToEntrance(Adventurer owner, SubGoal next) : base(owner, next) { }

        public override bool IsClear()
        {
            throw new System.NotImplementedException();
        }
    }
}
