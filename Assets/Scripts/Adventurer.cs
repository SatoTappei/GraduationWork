using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Adventurer : Character, 
        IStatusBarDisplayStatus, 
        IProfileWindowDisplayStatus, 
        ITalkable, 
        IGamePlayAIResource, 
        IRolePlayAIResource
    {
        [SerializeField] AdventurerSheet _adventurerSheet;

        public List<string> AvailableActions { get; protected set; }
        public SubGoal[] SubGoals { get; protected set; }
        public int CurrentSubGoalIndex { get; protected set; }
        public ExploreRecord ExploreRecord { get; protected set; }
        public Queue<string> ActionLog { get; protected set; }
        public int ElapsedTurn { get; protected set; }
        public int TreasureCount { get; protected set; }
        public int ItemCount { get; protected set; }
        public int DefeatCount { get; protected set; }
        public string[] Item { get; protected set; }
        public SharedInformation[] Information { get; protected set; }
        public SharedInformation TalkContent { get; set; }
        public int CurrentHp { get; protected set; }
        public int CurrentEmotion { get; protected set; }

        public AdventurerSheet AdventurerSheet => _adventurerSheet;
        public Sprite Icon => _adventurerSheet.Icon;
        public string FullName => _adventurerSheet.FullName;
        public string DisplayName => _adventurerSheet.DisplayName;
        public string Job => _adventurerSheet.Job;
        public string Background => _adventurerSheet.Background;
        public int MaxHp => 100;
        public int MaxEmotion => 100;
        public string Goal
        {
            get
            {
                if (SubGoals == null) return "--";
                if (CurrentSubGoalIndex < 0 || SubGoals.Length <= CurrentSubGoalIndex) return "--";
                if (SubGoals[CurrentSubGoalIndex] == null) return "--";

                return SubGoals[CurrentSubGoalIndex].Text.Japanese;
            }
        }

        IReadOnlyList<string> IProfileWindowDisplayStatus.Item => Item;
        IReadOnlyList<SharedInformation> IProfileWindowDisplayStatus.Information => Information;

        IReadOnlyExploreRecord IGamePlayAIResource.ExploreRecord => ExploreRecord;
        IReadOnlyCollection<string> IGamePlayAIResource.ActionLog => ActionLog;
        IReadOnlyList<SharedInformation> IGamePlayAIResource.Information => Information;
        IReadOnlyList<string> IGamePlayAIResource.AvailableActions => AvailableActions;
        IReadOnlyList<SubGoal> IGamePlayAIResource.SubGoals => SubGoals;

        public virtual void Talk(string id, BilingualString text, Vector2Int coords) { }
    }
}