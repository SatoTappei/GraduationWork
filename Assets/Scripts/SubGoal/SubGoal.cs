using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class SubGoal
    {
        public SubGoal(Adventurer owner)
        {
            Owner = owner;
        }

        public SubGoal(Adventurer owner, SubGoal next) : this(owner)
        {
            Next = next;
        }

        public abstract bool IsClear();

        public SubGoal Next { get; set; }
        protected Adventurer Owner { get; }
    }
}
