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
        public IReadOnlyList<SubGoal> SubGoals { get; }
        public int CurrentSubGoalIndex { get; }
        public Vector2Int Coords { get; }
    }
}
