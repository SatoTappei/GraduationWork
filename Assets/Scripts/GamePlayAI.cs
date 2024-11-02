using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class GamePlayAI
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

        IGamePlayAIResource _resource;
        AIRequest _ai;
        DungeonManager _dungeonManager;

        public GamePlayAI(IGamePlayAIResource resource)
        {
            _resource = resource;

            string personality = resource.AdventurerSheet.Personality;
            string motivation = resource.AdventurerSheet.Motivation;
            string weaknesses = resource.AdventurerSheet.Weaknesses;
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
            _ai = AIRequestFactory.Create(prompt);

            _dungeonManager = DungeonManager.Find();
        }

        public async UniTask<string> RequestNextActionAsync()
        {
            RequestFormat format = new RequestFormat();
            format.CurrentCoords = _resource.Coords;
            format.CurrentLocation = _dungeonManager.GetCell(_resource.Coords).Location.ToString();
            format.Surroundings.North = GetCellInfo(Vector2Int.up);
            format.Surroundings.South = GetCellInfo(Vector2Int.down);
            format.Surroundings.East = GetCellInfo(Vector2Int.right);
            format.Surroundings.West = GetCellInfo(Vector2Int.left);
            format.ActionLog = _resource.ActionLog.ToArray();
            format.DecisionSupportContext = _resource.Information
                .Where(info => info != null)
                .Select(info => info.Text.English)
                .ToArray();
            format.AvailableActions = _resource.AvailableActions.ToArray();
            format.Goal = _resource.SubGoals[_resource.CurrentSubGoalIndex].Text.English;

            try
            {
                return await _ai.RequestAsync(JsonUtility.ToJson(format));
            }
            catch (UnityWebRequestException e)
            {
                // 現在の仕様ではAvailableActionsの中から選ばなかった場合、再度リクエストするので適当な値を返しておく。
                return e.Message;
            }
        }

        string GetCellInfo(Vector2Int direction)
        {
            Vector2Int coords = _resource.Coords + direction;
            return GetCellInfo(coords, _resource.ExploreRecord);
        }

        string GetCellInfo(Vector2Int coords, IReadOnlyExploreRecord record)
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
                else if (actor is Enemy _)
                {
                    info = "Enemy";
                }
                else if (actor.ID == "Treasure")
                {
                    info = "There is a treasure chest. You can get it when you scavenge out the contents.";
                }
                else if (actor.ID == "Door")
                {
                    info = "Door";
                }
                else if (actor.ID == "Entrance")
                {
                    info = "Entrance";
                }
                else if (actor.ID == "Barrel")
                {
                    info = "There's a barrel. You might be able to obtain items or information by scavenging it.";
                }
                else if (actor.ID == "Container")
                {
                    info = "There's a container. You might be able to obtain items or information by scavenging it.";
                }
            }

            if (record != null)
            {
                string tag = GetExploreRecordTag(coords, record);
                return tag + info;
            }
            else
            {
                return info;
            }
        }

        // 探索回数に応じたタグを返す。
        static string GetExploreRecordTag(Vector2Int coords, IReadOnlyExploreRecord record)
        {
            int count = record.GetCount(coords);

            if (count == 0) return "[Unexplored] ";
            else return $"[Exproled {count} times.] ";
        }
    }
}