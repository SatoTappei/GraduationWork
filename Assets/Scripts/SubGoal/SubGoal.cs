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

        protected Adventurer Owner { get; }
        public abstract BilingualString Text { get; }

        public abstract bool IsCompleted();
        public virtual IEnumerable<string> GetAdditionalChoices() { yield break; }
    }
}
