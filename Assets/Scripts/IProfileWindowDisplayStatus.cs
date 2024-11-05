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
        public string Goal { get; }
        public IReadOnlyInventory ItemInventory { get; }
        public IReadOnlyList<SharedInformation> Information { get; }
    }
}
