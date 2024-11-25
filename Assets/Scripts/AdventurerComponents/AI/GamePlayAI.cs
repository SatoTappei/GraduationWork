using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    // Awake�̃^�C�~���O�ō��̒l���Q�Ƃ���̂ŁA���̏��������������悱�̃X�N���v�g��AddComponent����B
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
        AIRequest _ai;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _blackboard = GetComponent<Blackboard>();
            _actionLog = GetComponent<ActionLog>();
            _informationStock = GetComponent<InformationStock>();
            _availableActions = GetComponent<AvailableActions>();
            _subGoalPath = GetComponent<SubGoalPath>();
            _exploreRecord = GetComponent<ExploreRecord>();
            _dungeonManager = DungeonManager.Find();

            string personality = _blackboard.AdventurerSheet.Personality;
            string motivation = _blackboard.AdventurerSheet.Motivation;
            string weaknesses = _blackboard.AdventurerSheet.Weaknesses;
            string prompt =
                $"# Instructions\n" +
                $"- Your character�fs attributes are as follows. Consider these settings carefully when deciding the next action.\n" +
                $"- **Character Profile**: {personality}, {motivation}, {weaknesses}\n" +
                $"- Choose actions that align with the character�fs personality, motivations, and typical behavior patterns. Avoid actions that go against their personality or expose their weaknesses.\n" +
#if true
                $"- Select one of the AvailableActions and output the value only.";
#else
                // ���̍s����I���������R�A���ɕK�v�ȏ�񂪖������m�F����p�r�B
                $"- Please select the next action from the AvailableActions and tell us why you made that choice.\n" +
                $"- If you lack the information needed to select the next action, please tell us what information you want.";
#endif
            _ai = AIRequestFactory.Create(prompt);
        }

        public async UniTask<string> RequestNextActionAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

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

            try
            {
                return await _ai.RequestAsync(JsonUtility.ToJson(format));
            }
            catch (UnityWebRequestException)
            {
                // �u�������Ȃ��v��Ԃ��čēx���N�G�X�g���Ă��炤�B
                return "Idle";
            }
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

            return GetExploreRecordTag(coords) + info;
        }

        // �T���񐔂ɉ������^�O��Ԃ��B
        string GetExploreRecordTag(Vector2Int coords)
        {
            int count = _exploreRecord.GetCount(coords);

            if (count == 0) return "[Unexplored] ";
            else return $"[Exproled {count} times.] ";
        }
    }
}