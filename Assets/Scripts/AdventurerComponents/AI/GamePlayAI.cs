using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using AI;

namespace Game
{
    // Awakeのタイミングで黒板の値を参照するので、黒板の初期化が完了次第このスクリプトをAddComponentする。
    public class GamePlayAI : MonoBehaviour
    {
        [System.Serializable]
        class RequestFormat
        {
            public RequestFormat()
            {
                Surroundings = new Surroundings();
            }

            public Vector2Int CurrentCoords;
            public string CurrentLocation;
            public Surroundings Surroundings;
            public string[] ActionLog;
            public string[] DecisionSupportContext;
            public string[] AvailableActions;
            public string Goal;
        }

        [System.Serializable]
        class Surroundings
        {
            public string North;
            public string South;
            public string East;
            public string West;
        }

        Adventurer _adventurer;
        Blackboard _blackboard;
        ActionLog _actionLog;
        InformationStock _informationStock;
        AvailableActions _availableActions;
        SubGoalPath _subGoalPath;
        ExploreRecord _exploreRecord;
        DungeonManager _dungeonManager;
        AIClient _ai;

        // AIの挙動が気に入らない場合、次にリクエストする前に初期化する。
        bool _isRequestedInitialize;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _blackboard = GetComponent<Blackboard>();
            _actionLog = GetComponent<ActionLog>();
            _informationStock = GetComponent<InformationStock>();
            _availableActions = GetComponent<AvailableActions>();
            _subGoalPath = GetComponent<SubGoalPath>();
            _exploreRecord = GetComponent<ExploreRecord>();
            DungeonManager.TryFind(out _dungeonManager);

            Initialize();
        }

        public void RequestInitialize()
        {
            _isRequestedInitialize = true;
        }

        public async UniTask<string> RequestNextActionAsync(CancellationToken token)
        {
            RequestFormat format = new RequestFormat();
            format.CurrentCoords = _adventurer.Coords;
            format.CurrentLocation = _dungeonManager.GetCell(_adventurer.Coords).Location.ToString();
            format.Surroundings.North = GetCellInfo(_adventurer.Coords + Vector2Int.up);
            format.Surroundings.South = GetCellInfo(_adventurer.Coords + Vector2Int.down);
            format.Surroundings.East = GetCellInfo(_adventurer.Coords + Vector2Int.right);
            format.Surroundings.West = GetCellInfo(_adventurer.Coords + Vector2Int.left);
            format.ActionLog = _actionLog.Log.ToArray();
            format.DecisionSupportContext = _informationStock.Entries.ToArray();
            format.AvailableActions = _availableActions.Actions.ToArray();
            format.Goal = _subGoalPath.Current.Text.English;

            // 初期化を要求されていた場合。
            if (_isRequestedInitialize)
            {
                _isRequestedInitialize = false;
                Initialize();
            }

            string response = await _ai.RequestAsync(JsonUtility.ToJson(format), token);
            token.ThrowIfCancellationRequested();

            return response;
        }

        void Initialize()
        {
            string personality = _blackboard.AdventurerSheet.Personality;
            string motivation = _blackboard.AdventurerSheet.Motivation;
            string weaknesses = _blackboard.AdventurerSheet.Weaknesses;
            string prompt =
                $"# Instructions\n" +
                $"- Your character’s attributes are as follows. Consider these settings carefully when deciding the next action.\n" +
                $"- **Character Profile**: {personality}, {motivation}, {weaknesses}\n" +
                $"- Choose actions that align with the character’s personality, motivations, and typical behavior patterns. Avoid actions that go against their personality or expose their weaknesses.\n" +
#if true
                $"- Select one of the AvailableActions and output the value only.";
#else
                // その行動を選択した理由、他に必要な情報が無いか確認する用途。
                $"- Please select the next action from the AvailableActions and tell us why you made that choice.\n" +
                $"- If you lack the information needed to select the next action, please tell us what information you want.";
#endif
            _ai = new AIClient(prompt);
        }

        string GetCellInfo(Vector2Int coords)
        {
            if (Blueprint.Base[coords.y][coords.x] == '#') return "Wall";

            Cell cell = _dungeonManager.GetCell(coords);
            string info = "Floor";
            foreach (Actor actor in cell.GetActors())
            {
                if (actor is Adventurer adventurer)
                {
                    info = $"There is {adventurer.ID}, an adventurer like me.";
                }
                else if (actor is Enemy enemy)
                {
                    if (enemy.ID == nameof(BlackKaduki))
                    {
                        info = "There is an enemy that seems weaker than you. Do you attack?";
                    }
                    else if (enemy.ID == nameof(Soldier))
                    {
                        info = "There is an enemy who is as strong as you. Do you attack?";
                    }
                    else if (enemy.ID == nameof(Golem))
                    {
                        info = "In front of you is the boss of the dungeon. Do you attack?";
                    }
                }
                else if (actor is Treasure treasure)
                {
                    if (treasure.IsEmpty)
                    {
                        info = "There is a treasure chest. But Is Empty.";
                    }
                    else
                    {
                        info = "There is a treasure chest. You can get it when you scavenge out the contents.";
                    }
                }
                else if (actor.ID == nameof(Barrel))
                {
                    info = "There's a barrel. You might be able to obtain items or information by scavenging it.";
                }
                else if (actor.ID == nameof(Container))
                {
                    info = "There's a container. You might be able to obtain items or information by scavenging it.";
                }
                else if (actor.ID == nameof(HealingSpot))
                {
                    info = "There are places where you can recover your health.";
                }
                else if (actor.ID == nameof(Lever))
                {
                    info = "There's a lever. To activate the mechanism, select Scavenge.";
                }
                else if (cell.TerrainEffect == TerrainEffect.Flaming)
                {
                    info = "The floor is on fire. If you step on the floor, you will get burned.";
                }
                else
                {
                    // 扉、入口、その他はそのままIDを返す。
                    info = actor.ID;
                }
            }

            return GetExploreRecordTag(coords) + info;
        }

        // 探索回数に応じたタグを返す。
        string GetExploreRecordTag(Vector2Int coords)
        {
            int count = _exploreRecord.GetCount(coords);

            if (count == 0) return "[Unexplored] ";
            else return $"[Exproled {count} times.] ";
        }
    }
}