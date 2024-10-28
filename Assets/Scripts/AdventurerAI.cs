using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // AIは宝箱の隣に立ってSurroundingを選ぶ…という宝箱の開け方を知らない。
    // AIは敵の攻撃が成功=敵を倒したと思っているので1回しか攻撃しない。
    // AIはダンジョンの入口を探す時も未探索のセルを優先的に探索してしまう。
    //  プログラム側で入口に戻るようなセルのスコアを上げてしまうのはルールベースと同じになってしまう。
    //  AI側が入口に戻るようなセルのスコアを上げるようにならないか？
    //  BluePrint.Baseを渡してダンジョンのマップ代わりにならないだろうか？
    // ★サブゴールの達成具合でAvailableActionsを解禁する方式にする。
    //    例えば宝箱を見つけるまでは東西南北への移動のみ、宝箱を見つけた場合は入口に戻るというアクションを追加してやる。
    //    そうすればアクションのスコア付けによって戻るもしくはまだ探索するみたいな選択をAI側が出来るかも？
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

        // 非同期処理で初期化。
        async UniTask InitializeAsync()
        {
            CreateRolePlayAI();
            await CreateSubGoalAsync();
            CreateGamePlayAI();

            _isInitialized = true;
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
                // 現在の仕様ではAvailableActionsの中から選ばなかった場合、再度リクエストするので適当な値を返しておく。
                response = e.Message;
            }
#if true
            Debug.Log("AIの判断: " + response);
#endif
            return response;
        }

        // 行動の結果を報告。
        public void ReportActionResult(string result)
        {
            _actionLog ??= new Queue<string>();
            _actionLog.Enqueue(result);

            if (_actionLog.Count > 10) _actionLog.Dequeue();
        }

        // 探索されたセルを報告。
        public void ReportExploredCell(Vector2Int coords)
        {
            _explored[coords.y, coords.x]++;
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
#if true
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