using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IProfileWindowDisplayStatus
    {
        public string FullName { get; }
        public string Job { get; }
        public string Background { get; }
        public SubGoal CurrentSubGoal { get; }
        public IEnumerable<ItemInventory.Entry> Item { get; }
        public IReadOnlyList<SharedInformation> Information { get; }
    }
}
