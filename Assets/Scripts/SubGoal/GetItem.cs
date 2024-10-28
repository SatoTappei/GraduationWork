using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GetItem : SubGoal
    {
        public GetItem(Adventurer owner) : base(owner) { }
        public GetItem(Adventurer owner, SubGoal next) : base(owner, next) { }

        public override bool IsClear()
        {
            throw new System.NotImplementedException();
        }
    }
}