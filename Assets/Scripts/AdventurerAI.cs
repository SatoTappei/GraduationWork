using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // AI�͕󔠂ׂ̗ɗ�����Surrounding��I�ԁc�Ƃ����󔠂̊J������m��Ȃ��B
    // AI�͓G�̍U��������=�G��|�����Ǝv���Ă���̂�1�񂵂��U�����Ȃ��B
    // AI�̓_���W�����̓�����T���������T���̃Z����D��I�ɒT�����Ă��܂��B
    //  �v���O�������œ����ɖ߂�悤�ȃZ���̃X�R�A���グ�Ă��܂��̂̓��[���x�[�X�Ɠ����ɂȂ��Ă��܂��B
    //  AI���������ɖ߂�悤�ȃZ���̃X�R�A���グ��悤�ɂȂ�Ȃ����H
    //  BluePrint.Base��n���ă_���W�����̃}�b�v����ɂȂ�Ȃ����낤���H
    // ���T�u�S�[���̒B�����AvailableActions�����ւ�������ɂ���B
    //    �Ⴆ�Ε󔠂�������܂ł͓�����k�ւ̈ړ��̂݁A�󔠂��������ꍇ�͓����ɖ߂�Ƃ����A�N�V������ǉ����Ă��B
    //    ��������΃A�N�V�����̃X�R�A�t���ɂ���Ė߂�������͂܂��T������݂����ȑI����AI�����o���邩���H
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

        [SerializeField] AdventurerSheet _adventurerSheet;

        DungeonManager _dungeonManager;
        Adventurer _adventurer;
        GptRequest _rolePlayAI;
        GptRequest _gamePlayAI;
        Queue<string> _actionLog;
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
            _availableActions = new List<string>()
            {
                "Move North",
                "Move South",
                "Move East",
                "Move West",
                "Attack Surrounding",
                "Scavenge Surrounding"
            };
            _explored = new int[Blueprint.Height, Blueprint.Width];
        }

        void Start()
        {
            InitializeAsync().Forget();
        }

        // �񓯊������ŏ������B
        async UniTask InitializeAsync()
        {
            CreateRolePlayAI();
            await CreateSubGoalAsync();
            CreateGamePlayAI();

            _isInitialized = true;
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
            format.Rumors = new string[]
            {
                "Attack and defeat Kaduki."
            };
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
            return response;
        }

        // �s���̌��ʂ�񍐁B
        public void ReportActionResult(string result)
        {
            _actionLog ??= new Queue<string>();
            _actionLog.Enqueue(result);

            if (_actionLog.Count > 10) _actionLog.Dequeue();
        }

        // �T�����ꂽ�Z����񍐁B
        public void ReportExploredCell(Vector2Int coords)
        {
            _explored[coords.y, coords.x]++;
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
#if true
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
                if (actor is Adventurer adventurer) return tag + adventurer.ID;
                if (actor is Enemy _) return tag + "Enemy";
                if (actor.ID == "Treasure") return tag + "Treasure";
                if (actor.ID == "Door") return tag + "Door";
                if (actor.ID == "Entrance") return tag + "Entrance";
            }

            return tag + "Floor";
        }
    }
}