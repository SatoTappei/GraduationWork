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
            
            // 最初の1回はデフォルトで保持している噂から話題を選ぶ。
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

        // ChatGPTのAPIに次の行動を選択させる。
        public async UniTask<string> SelectNextActionAsync()
        {
            // Startのタイミングで初期化開始しているので、終わるまで待つ。
            await UniTask.WaitUntil(() => _isInitialized);

            if (IsSubGoalCompleted(_subGoals[_currentSubGoalIndex]))
            {
                _currentSubGoalIndex++;
                
                if (_subGoals[_currentSubGoalIndex] == ReturnToEntrance)
                {
                    Debug.Log("入口に戻る選択肢追加！");
                    _availableActions.Add("Return To Entrance");
                }
            }
#if false
            Debug.Log("現在のサブゴール: " + _subGoals[_currentSubGoalIndex]);
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
                // 現在の仕様ではAvailableActionsの中から選ばなかった場合、再度リクエストするので適当な値を返しておく。
                response = e.Message;
            }
#if true
            Debug.Log("AIの判断: " + response);
#endif

            _selectedAction = response;

            return response;
        }

        // AIが選択した行動の結果を報告してもらう。
        public void ReportActionResult(string result)
        {
            _actionLog ??= new Queue<string>();
            _actionLog.Enqueue(result);

            if (_actionLog.Count > 10) _actionLog.Dequeue();
        }

        // AIが移動を選択し、実際にセルを移動した場合は報告してもらう。
        public void ReportExploredCell(Vector2Int coords)
        {
            _explored[coords.y, coords.x]++;
        }

        // 会話の内容を取得。
        public string GetTopic()
        {
            return _talkTopic;
        }

        // 会話相手から聞いた噂をAIが判定。
        public void SetRumor(string source, string message)
        {
            if (message == string.Empty) return;

            _unevaluatedRumors.Enqueue(new Rumor() { Source = source, Message = message });
        }

        // キャラクターとして振る舞うAIは、台詞や背景などをUIに表示するので日本語。
        void CreateRolePlayAI()
        {
            string prompt =
                $"# 指示内容\n" +
                $"- 以下のキャラクターになりきって各質問に答えてください。\n" +
                $"'''\n" +
                $"# キャラクター\n" +
                $"- {_adventurerSheet.Age}歳の{_adventurerSheet.Job}。\n" +
                $"- {_adventurerSheet.Background}\n";

            _rolePlayAI = GptRequestFactory.Create(prompt);
        }

        // ゲームをプレイするAIは、リクエスト回数が多く、トークン数が有利な英語。
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
                // その行動を選択した理由、他に必要な情報が無いか確認する用途。
                $"- Please select the next action from the AvailableActions and tell us why you made that choice.\n" +
                $"- If you lack the information needed to select the next action, please tell us what information you want.";
#endif
            _gamePlayAI = GptRequestFactory.Create(prompt);
        }

        // キャラクターの背景を基に、ダンジョン内で達成すべきサブゴールを生成。
        async UniTask CreateSubGoalAsync()
        {
            string prompt =
                $"# 指示内容\n" +
                $"- キャラクターを冒険者としてダンジョン探索ゲームに登場させます。\n" +
                $"- 自身のキャラクターの設定を基に、ゲームクリアまでに必要なサブゴールを選択してください。\n" +
                $"- 以下の選択肢から合計2つ選択してください。\n" +
                $"'''\n" +
                $"# 選択肢\n" +
                $"- {GetTreasure} 0\n" +
                $"- {GetItem} 1\n" +
                $"- {WalkDungeon} 2\n" +
                $"- {DefeatWeakEnemy} 3\n" +
                $"- {DefeatStrongEnemy} 4\n" +
                $"- {DefeatBossEnemy} 5\n" +
                $"- {DefeatAdventurer} 6\n" +
                $"'''\n" +
                $"# 出力形式\n" +
#if true
                $"- 各選択肢の末尾の番号のみを半角スペース区切りで出力してください。\n" +
#else
                // キャラクターの背景と照らし合わせて確認する用途。
                $"- 各選択肢の末尾の番号と、その選択をした理由を出力してください。\n" +
#endif
                $"'''\n" +
                $"# 出力例\n" +
                $"- 1 3\n" +
                $"- 4 7\n";

            string response = await _rolePlayAI.RequestAsync(prompt);

#if false
            // AIからのレスポンスが出力例とは異なる場合を想定し、文字列から数値のみを抽出する。
            // 最後は全員共通で"入口に戻る"なので、リストの末尾に対応する番号である7を追加している。
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
            foreach (string s in _subGoals) Debug.Log("設定したサブゴール: " + s);
#endif
        }

        // 現在のサブゴールを達成したかチェック。
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

            Debug.LogError($"番号に対応するサブゴールが無い。: {subGoal}");

            return false;
        }

        // セルの情報を返す。
        string GetCellInfo(Vector2Int coords)
        {
            if (Blueprint.Base[coords.y][coords.x] == '#') return "Wall";

            // 既に訪れたセルかどうかをマークしておく。
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

        // 噂をAIが評価し、スコア順に並べる。
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

        // 自身の保持している噂から、会話する際に相手に伝える噂を選ぶ。
        async UniTask UpdateTalkTopicAsync()
        {
            // 保持している噂の中身が空文字しか無い場合はAIが正常に判断できない可能性がある。
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

            // 他の冒険者に伝える内容を判断する基準はAI任せなので、ヒントより挨拶を優先してしまうことがある。
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