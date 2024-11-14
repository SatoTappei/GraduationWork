using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // AIがロールプレイや次の行動を選択するために必要な情報を参照することが出来る。
    public interface IReadOnlyAdventurerContext : IStatusBarDisplayStatus, IProfileWindowDisplayStatus
    {
        public AdventurerSheet AdventurerSheet { get; }
        public SubGoal CurrentSubGoal { get; }
        public IReadOnlyExploreRecord ExploreRecord { get; }
        public IReadOnlyList<string> AvailableActions { get; }
        public IReadOnlyCollection<string> ActionLog { get; }
        public Vector2Int Coords { get; }
        public int DefeatCount { get; }
        public int TreasureCount { get; }
        public int ElapsedTurn { get; }
    }
}
