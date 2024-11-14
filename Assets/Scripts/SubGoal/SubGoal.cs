using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class SubGoal
    {
        public SubGoal(IReadOnlyAdventurerContext context)
        {
            Context = context;
        }

        protected IReadOnlyAdventurerContext Context { get; }
        public abstract BilingualString Text { get; }

        public abstract bool IsCompleted();
        public virtual IEnumerable<string> GetAdditionalActions() { yield break; }
    }
}
