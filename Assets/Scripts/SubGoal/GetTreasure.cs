using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GetTreasure : SubGoal
    {
        public GetTreasure(Adventurer owner) : base(owner) { }
        public GetTreasure(Adventurer owner, SubGoal next) : base(owner, next) { }

        public override bool IsClear()
        {
            throw new System.NotImplementedException();
        }
    }
}