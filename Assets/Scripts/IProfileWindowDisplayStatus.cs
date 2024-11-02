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
        public IReadOnlyList<string> Item { get; }
        public IReadOnlyList<SharedInformation> Information { get; }
    }
}
