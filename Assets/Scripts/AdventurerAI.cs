using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class AdventurerAI : MonoBehaviour
    {
        const string GetTreasure = "Find the treasure chest in the dungeon and scavenge.";
        const string GetItem = "Bring back the requested items.";
        const string WalkDungeon = "Take a walk in the dungeon.";
        const string DefeatWeakEnemy = "Defeat weak enemies roaming in the dungeon";
        const string DefeatStrongEnemy = "Defeat strong enemies roaming in the dungeon";
        const string DefeatBossEnemy = "Defeat the dungeon boss.";
        const string DefeatAdventurer = "Defeat the adventurers.";
        const string ReturnToEntrance = "Return to the entrance.";

        static readonly string[] Hints =
        {
            "The door on the north side of the entrance intrigues me.",
            "The door on the south side of the entrance intrigues me.",
            "The door on the east side of the entrance intrigues me.",
            "The door on the west side of the entrance intrigues me.",
        };

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
            public string[] Rumors;
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

        [System.Serializable]
        class Rumor
        {
            public string Message;
            public string Source;
            public float Score;
        }

        [System.Serializable]
        class TalkTopicChoices
        {
            public string[] Value;
        }

        [SerializeField] AdventurerSheet _adventurerSheet;
        [Space(10)]
        [SerializeField] List<Rumor> _rumors;
        [SerializeField] string _talkTopic;
        [SerializeField] string _selectedAction;

        DungeonManager _dungeonManager;
        Adventurer _adventurer;
        GptRequest _rolePlayAI;
        GptRequest _gamePlayAI;
        Queue<string> _actionLog;
        Queue<Rumor> _unevaluatedRumors;
        List<string> _availableActions;
        string[] _subGoals;
        int _currentSubGoalIndex;
        int[,] _explored;
        
        bool _isInitialized;

        void Awake()
        {
            _dungeonManager = DungeonManager.Find();
            _adventurer = GetComponent<Adventurer>();
            _actionLog = new Queue<string>();
            _unevaluatedRumors = new Queue<Rumor>();
            _rumors = new List<Rumor>()
            {
               new Rumor(){ Message = Hints[Random.Range(0, Hints.Length)], Source = "Spoiler" }
            };
            _availableActions = new List<string>()
            {
                "Move Forward",
                "Move North",
                "Move South",
                "Move East",
                "Move West",
                "Attack Surrounding",
                "Scavenge Surrounding",
                "Talk Surrounding"
            };
            _explored = new int[Blueprint.Height, Blueprint.Width];
        }

        void Start()
        {
            UpdateAsync().Forget();
        }

        async UniTask UpdateAsync()
        {
            CreateRolePlayAI();
            await CreateSubGoalAsync();
            CreateGamePlayAI();
            
            // �ŏ���1��̓f�t�H���g�ŕێ����Ă���\����b���I�ԁB
            await UpdateTalkTopicAsync();

            _isInitialized = true;

            while (true)
            {
                if (_unevaluatedRumors.TryDequeue(out Rumor info))
                {
                    await EvaluateRumorsAsync(info.Source, info.Message);
                    await UpdateTalkTopicAsync();
                }

                await UniTask.Yield();
            }
        }

        // ChatGPT��API�Ɏ��̍s����I��������B
        public async UniTask<string> SelectNextActionAsync()
        {
            // Start�̃^�C�~���O�ŏ������J�n���Ă���̂ŁA�I���܂ő҂B
            await UniTask.WaitUntil(() => _isInitialized);

            if (IsSubGoalCompleted(_subGoals[_currentSubGoalIndex]))
            {
                _currentSubGoalIndex++;
                
                if (_subGoals[_currentSubGoalIndex] == ReturnToEntrance)
                {
                    Debug.Log("�����ɖ߂�I�����ǉ��I");
                    _availableActions.Add("Return To Entrance");
                }
            }
#if false
            Debug.Log("���݂̃T�u�S�[��: " + _subGoals[_currentSubGoalIndex]);
#endif
            RequestFormat format = new RequestFormat();
            format.CurrentCoords = _adventurer.Coords;
            format.CurrentLocation = _dungeonManager.GetCell(_adventurer.Coords).Location.ToString();
            format.Surroundings.North = GetCellInfo(_adventurer.Coords + Vector2Int.up);
            format.Surroundings.South = GetCellInfo(_adventurer.Coords + Vector2Int.down);
            format.Surroundings.East = GetCellInfo(_adventurer.Coords + Vector2Int.right);
            format.Surroundings.West = GetCellInfo(_adventurer.Coords + Vector2Int.left);
            format.ActionLog = _actionLog.ToArray();
            format.Rumors = _rumors.Select(r => r.Message).ToArray();
            format.AvailableActions = _availableActions.ToArray();
            format.Goal = _subGoals[_currentSubGoalIndex];

            string response;
            try
            {
                response = await _gamePlayAI.RequestAsync(JsonUtility.ToJson(format));
            }
            catch(UnityWebRequestException e)
            {
                // ���݂̎d�l�ł�AvailableActions�̒�����I�΂Ȃ������ꍇ�A�ēx���N�G�X�g����̂œK���Ȓl��Ԃ��Ă����B
                response = e.Message;
            }
#if true
            Debug.Log("AI�̔��f: " + response);
#endif

            _selectedAction = response;

            return response;
        }

        // AI���I�������s���̌��ʂ�񍐂��Ă��炤�B
        public void ReportActionResult(string result)
        {
            _actionLog ??= new Queue<string>();
            _actionLog.Enqueue(result);

            if (_actionLog.Count > 10) _actionLog.Dequeue();
        }

        // AI���ړ���I�����A���ۂɃZ�����ړ������ꍇ�͕񍐂��Ă��炤�B
        public void ReportExploredCell(Vector2Int coords)
        {
            _explored[coords.y, coords.x]++;
        }

        // ��b�̓��e���擾�B
        public string GetTopic()
        {
            return _talkTopic;
        }

        // ��b���肩�畷�����\��AI������B
        public void SetRumor(string source, string message)
        {
            if (message == string.Empty) return;

            _unevaluatedRumors.Enqueue(new Rumor() { Source = source, Message = message });
        }

        // �L�����N�^�[�Ƃ��ĐU�镑��AI�́A�䎌��w�i�Ȃǂ�UI�ɕ\������̂œ��{��B
        void CreateRolePlayAI()
        {
            string prompt =
                $"# �w�����e\n" +
                $"- �ȉ��̃L�����N�^�[�ɂȂ肫���Ċe����ɓ����Ă��������B\n" +
                $"'''\n" +
                $"# �L�����N�^�[\n" +
                $"- {_adventurerSheet.Age}�΂�{_adventurerSheet.Job}�B\n" +
                $"- {_adventurerSheet.Background}\n";

            _rolePlayAI = GptRequestFactory.Create(prompt);
        }

        // �Q�[�����v���C����AI�́A���N�G�X�g�񐔂������A�g�[�N�������L���ȉp��B
        void CreateGamePlayAI()
        {
            string prompt =
                $"# Instructions\n" +
                $"- You are a player in a game of dungeon exploration.\n" +
                $"- Determine which action to choose next to achieve the Goal.\n" +
                $"- The current status is given in Json format.\n" +
#if true
                $"- Select one of the AvailableActions and output the value only.\n";
#else
                // ���̍s����I���������R�A���ɕK�v�ȏ�񂪖������m�F����p�r�B
                $"- Please select the next action from the AvailableActions and tell us why you made that choice.\n" +
                $"- If you lack the information needed to select the next action, please tell us what information you want.";
#endif
            _gamePlayAI = GptRequestFactory.Create(prompt);
        }

        // �L�����N�^�[�̔w�i����ɁA�_���W�������ŒB�����ׂ��T�u�S�[���𐶐��B
        async UniTask CreateSubGoalAsync()
        {
            string prompt =
                $"# �w�����e\n" +
                $"- �L�����N�^�[��`���҂Ƃ��ă_���W�����T���Q�[���ɓo�ꂳ���܂��B\n" +
                $"- ���g�̃L�����N�^�[�̐ݒ����ɁA�Q�[���N���A�܂łɕK�v�ȃT�u�S�[����I�����Ă��������B\n" +
                $"- �ȉ��̑I�������獇�v2�I�����Ă��������B\n" +
                $"'''\n" +
                $"# �I����\n" +
                $"- {GetTreasure} 0\n" +
                $"- {GetItem} 1\n" +
                $"- {WalkDungeon} 2\n" +
                $"- {DefeatWeakEnemy} 3\n" +
                $"- {DefeatStrongEnemy} 4\n" +
                $"- {DefeatBossEnemy} 5\n" +
                $"- {DefeatAdventurer} 6\n" +
                $"'''\n" +
                $"# �o�͌`��\n" +
#if true
                $"- �e�I�����̖����̔ԍ��݂̂𔼊p�X�y�[�X��؂�ŏo�͂��Ă��������B\n" +
#else
                // �L�����N�^�[�̔w�i�ƏƂ炵���킹�Ċm�F����p�r�B
                $"- �e�I�����̖����̔ԍ��ƁA���̑I�����������R���o�͂��Ă��������B\n" +
#endif
                $"'''\n" +
                $"# �o�͗�\n" +
                $"- 1 3\n" +
                $"- 4 7\n";

            string response = await _rolePlayAI.RequestAsync(prompt);

#if false
            // AI����̃��X�|���X���o�͗�Ƃ͈قȂ�ꍇ��z�肵�A�����񂩂琔�l�݂̂𒊏o����B
            // �Ō�͑S�����ʂ�"�����ɖ߂�"�Ȃ̂ŁA���X�g�̖����ɑΉ�����ԍ��ł���7��ǉ����Ă���B
            List<int> numbers = response
                .Split()
                .Where(s => int.TryParse(s, out int _))
                .Select(t => int.Parse(t))
                .ToList();
            numbers.Add(7);
#endif
            List<int> numbers = new List<int> { 0, 7, -1 };

            _subGoals = new string[3];
            for (int i = 0; i < numbers.Count; i++)
            {
                if (numbers[i] == 0) _subGoals[i] = GetTreasure;
                else if (numbers[i] == 1) _subGoals[i] = GetItem;
                else if (numbers[i] == 2) _subGoals[i] = WalkDungeon;
                else if (numbers[i] == 3) _subGoals[i] = DefeatWeakEnemy;
                else if (numbers[i] == 4) _subGoals[i] = DefeatStrongEnemy;
                else if (numbers[i] == 5) _subGoals[i] = DefeatBossEnemy;
                else if (numbers[i] == 6) _subGoals[i] = DefeatAdventurer;
                else if (numbers[i] == 7) _subGoals[i] = ReturnToEntrance;
            }

            _currentSubGoalIndex = 0;
#if false
            foreach (string s in _subGoals) Debug.Log("�ݒ肵���T�u�S�[��: " + s);
#endif
        }

        // ���݂̃T�u�S�[����B���������`�F�b�N�B
        bool IsSubGoalCompleted(string subGoal)
        {
            if (subGoal == GetTreasure)
            {
                return _adventurer.TreasureCount > 0;
            }
            if (subGoal == GetItem)
            {
                return _adventurer.ItemCount > 0;
            }
            if (subGoal == WalkDungeon)
            {
                return _adventurer.ElapsedTurn > 10;
            }
            if (subGoal == DefeatWeakEnemy)
            {
                return _adventurer.DefeatCount > 0;
            }
            if (subGoal == DefeatStrongEnemy)
            {
                return _adventurer.DefeatCount > 0;
            }
            if (subGoal == DefeatBossEnemy)
            {
                return _adventurer.DefeatCount > 0;
            }
            if (subGoal == DefeatAdventurer)
            {
                return _adventurer.DefeatCount > 0;
            }
            if (subGoal == ReturnToEntrance)
            {
                return Blueprint.Interaction[_adventurer.Coords.y][_adventurer.Coords.x] == '<';
            }

            Debug.LogError($"�ԍ��ɑΉ�����T�u�S�[���������B: {subGoal}");

            return false;
        }

        // �Z���̏���Ԃ��B
        string GetCellInfo(Vector2Int coords)
        {
            if (Blueprint.Base[coords.y][coords.x] == '#') return "Wall";

            // ���ɖK�ꂽ�Z�����ǂ������}�[�N���Ă����B
            string tag;
            if (_explored[coords.y, coords.x] == 0) tag = "[Unexplored] ";
            else tag = $"[Exproled {_explored[coords.y, coords.x]} times.] ";

            Cell cell = _dungeonManager.GetCell(coords);
            foreach (Actor actor in cell.GetActors())
            {
                if (actor is Adventurer adventurer) return tag + $"There is {adventurer.ID}, an adventurer like me.";
                if (actor is Enemy _) return tag + "Enemy";
                if (actor.ID == "Treasure") return tag + "There is a treasure chest. You can get it when you scavenge out the contents.";
                if (actor.ID == "Door") return tag + "Door";
                if (actor.ID == "Entrance") return tag + "Entrance";
                if (actor.ID == "Barrel") return tag + "There's a barrel. You might be able to obtain items or information by scavenging it.";
                if (actor.ID == "Container") return tag + "There's a container. You might be able to obtain items or information by scavenging it.";
            }

            return tag + "Floor";
        }

        // �\��AI���]�����A�X�R�A���ɕ��ׂ�B
        async UniTask EvaluateRumorsAsync(string source, string message)
        {
            string prompt =
                $"# Instructions\n" +
                $"- You are a player in a game of dungeon exploration.\n" +
                $"- The combination of information and source is given in Json format.\n" +
                $"- It determines if the information is reliable or not and outputs only the confidence level with a value between 0 and 1.\n" +
                $"'''\n" +
                $"# OutputExample\n" +
                $"- 0.2\n" +
                $"- 1.0\n";
            GptRequest ai = GptRequestFactory.Create(prompt);
            Rumor rumor = new Rumor() { Message = message, Source = source };
            string result = await ai.RequestAsync(JsonUtility.ToJson(rumor));

            foreach (string s in result.Split())
            {
                if (float.TryParse(s, out rumor.Score)) break;
            }

            _rumors.Add(rumor);

            Sort(_rumors, 0, _rumors.Count - 1);

            if (_rumors.Count > 4)
            {
                _rumors.RemoveAt(_rumors.Count - 1);
            }
        }

        // ���g�̕ێ����Ă���\����A��b����ۂɑ���ɓ`����\��I�ԁB
        async UniTask UpdateTalkTopicAsync()
        {
            // �ێ����Ă���\�̒��g���󕶎����������ꍇ��AI������ɔ��f�ł��Ȃ��\��������B
            bool isEmpty = true;
            foreach (Rumor r in _rumors)
            {
                if (r.Message != string.Empty)
                {
                    isEmpty = false;
                    break;
                }
            }

            if (isEmpty) return;

            // ���̖`���҂ɓ`������e�𔻒f������AI�C���Ȃ̂ŁA�q���g��舥�A��D�悵�Ă��܂����Ƃ�����B
            string prompt =
                $"# Instructions\n" +
                $"- You are a player in a game of dungeon exploration.\n" +
                $"- Some sentences are given in Json format.\n" +
                $"- A sentence deemed appropriate to communicate to other players is output as is.";
            GptRequest ai = GptRequestFactory.Create(prompt);
            TalkTopicChoices choices = new TalkTopicChoices();
            choices.Value = _rumors.Select(r => r.Message).ToArray();
            _talkTopic = await ai.RequestAsync(JsonUtility.ToJson(choices));
        }

        static void Sort(List<Rumor> array, int left, int right)
        {
            if (left >= right) return;

            float pivot = array[right].Score;
            int current = left;
            for (int i = left; i <= right - 1; i++)
            {
                if (array[i].Score > pivot)
                {
                    Swap(array, current, i);
                    current++;
                }
            }

            Swap(array, current, right);

            Sort(array, left, current - 1);
            Sort(array, current + 1, right);
        }

        static void Swap(List<Rumor> array, int a, int b)
        {
            Rumor x = array[a];
            array[a] = array[b];
            array[b] = x;
        }
    }
}