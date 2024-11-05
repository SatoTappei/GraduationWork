using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IGamePlayAIResource
    {
        public AdventurerSheet AdventurerSheet { get; }
        public IReadOnlyExploreRecord ExploreRecord { get; }
        public IReadOnlyCollection<string> ActionLog { get; }
        public IReadOnlyList<SharedInformation> Information { get; }
        public IReadOnlyList<string> AvailableActions { get; }
        public SubGoal CurrentSubGoal { get; }
        public Vector2Int Coords { get; }
    }
}
