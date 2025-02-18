using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using AI;
using Game.ItemData;
using System.IO;

namespace Game
{
    public class GamePlay : MonoBehaviour
    {
        [System.Serializable]
        class RequestFormat
        {
            public RequestFormat()
            {
                Surroundings = new Surroundings();
            }

            public string CurrentLocation;
            public Surroundings Surroundings;
            public string[] Items;
            public string[] ActionLog;
            public string[] Information;
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
        HoldInformation _information;
        AvailableActions _actions;
        SubGoalPath _subGoalPath;
        ItemInventory _item;
        AIClient _ai;

        // 次にリクエストする際、事前にAIを初期化するフラグ。
        bool _isPreInitialize;
        // FTのログを取る用。情報を整理せずに冒険を終えた場合に取る。
        bool _isHelped;
        List<string> _logs;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _information = GetComponent<HoldInformation>();
            _actions = GetComponent<AvailableActions>();
            _subGoalPath = GetComponent<SubGoalPath>();
            _item = GetComponent<ItemInventory>();

            _logs = new List<string>();
        }

#if UNITY_EDITOR
        void OnDestroy()
        {
            if (_isHelped) return;

            StreamWriter writer = new StreamWriter($"./Assets/Logs/ActionLog_{System.Guid.NewGuid()}.txt", false);
            foreach (string s in _logs)
            {
                writer.WriteLine(s);
            }
            writer.Flush();
            writer.Close();
        }
#endif

        public void PreInitialize()
        {
            _isPreInitialize = true;
        }

        public async UniTask<string> RequestAsync(CancellationToken token)
        {
            RequestFormat format = new RequestFormat();
             
            // 現在地の名称。
            format.CurrentLocation = DungeonManager.GetCell(_adventurer.Coords).Location.ToString();

            // 上下左右のセルに何があるか、探索済みなのかを送信。
            format.Surroundings.North = GetCellInfo(_adventurer.Coords + Vector2Int.up);
            format.Surroundings.South = GetCellInfo(_adventurer.Coords + Vector2Int.down);
            format.Surroundings.East = GetCellInfo(_adventurer.Coords + Vector2Int.right);
            format.Surroundings.West = GetCellInfo(_adventurer.Coords + Vector2Int.left);

            // アイテム。種類ごとにリストで管理しているので、それぞれのリストの先頭の情報を渡す。
            List<string> items = new List<string>();
            foreach (IReadOnlyList<Item> e in _item.Get().Values)
            {
                items.Add($"{e[0].Name.English} (Usage: {e[0].Usage})");
            }
            if (items.Count == 0) items.Add("None");
            format.Items = items.ToArray();

            // 行動ログ。
            format.ActionLog = _adventurer.ActionLog.Log.ToArray();
            
            // 情報。
            format.Information = _information.Information.Select(info => info.Text.English).ToArray();
            
            // この中から1つ行動を選ぶ。
            format.AvailableActions = _actions.GetEntries().ToArray();
            
            // 現在のサブゴール。
            format.Goal = _subGoalPath.GetCurrent().Description.English;

            // 初期化を要求されていた場合。
            if (_isPreInitialize)
            {
                _isPreInitialize = false;
                Initialize();
            }
            
            // 初期化を忘れて呼ばれた場合。
            if (_ai == null)
            {
                Debug.LogWarning("初期化せずに台詞をリクエストしたので、リクエスト前に初期化した。");
                Initialize();
            }

            string json = JsonUtility.ToJson(format, prettyPrint: true);
            string response = await _ai.RequestAsync(json, token);
            token.ThrowIfCancellationRequested();

            // 選択肢とスコアがスペース区切りで返ってくることを想定。
            // ダブルクオーテーションが付いている場合もある。
            string result = response.Split()[0].Trim('"');

            // ログに追加。
            _logs.Add($"{json}\n{result}");
            if (result == "RequestHelp") _isHelped = true;

            return result;
        }

        void Initialize()
        {
            if (_adventurer.Sheet == null)
            {
                Debug.LogWarning("冒険者のデータが読み込まれていない。");

                string model = "ft:gpt-4o-mini-2024-07-18:personal::AyDu9Zhp";
                string prompt = $"Select one of the AvailableActions and output the value only.";
                _ai = new AIClient(model, prompt);
            }
            else
            {
                string prompt =
                    $"# Instructions\n" +
                    $"- You are an adventurer in a roguelike game.\n" +
                    $"- Your objective is to achieve the goal.\n" +
                    $"- Prioritize exploring unexplored areas.\n" +
                    $"- Defeating enemies may grant you items.\n" +
                    $"- Scavenge chests and containers may yield items.\n" +
                    $"- You can choose to interact with other players either friendly or aggressively.\n" +
                    $"# OutputFormat\n" +
#if true
                    $"- Select one of the AvailableActions and output the value only.";
#else
                // その行動を選択した理由、他に必要な情報が無いか確認する用途。
                $"- Select one of the AvailableActions.\n" +
                $"- The reason for choosing that action is also output.\n" +
                $"- Is there any other information you would like to know when choosing an action?";
#endif
                _ai = new AIClient(prompt);
            }
        }

        string GetCellInfo(Vector2Int coords)
        {
            // 壁の場合。
            if (Blueprint.Base[coords.y][coords.x] == '#') return "Wall";

            // その座標に何がいるかを説明する。
            Cell cell = DungeonManager.GetCell(coords);
            string info = "Floor";
            foreach (Actor actor in cell.GetActors())
            {
                if (actor.TryGetComponent(out Adventurer _))
                {
                    info = $"There is an adventurer exploring a dungeon.";
                }
                else if (actor.TryGetComponent(out Enemy enemy))
                {
                    if (enemy.ID == nameof(BlackKaduki))
                    {
                        info = "There is an enemy in the form of a girl. She is hostile. Will you attack her?";
                    }
                    else if (enemy.ID == nameof(Soldier))
                    {
                        info = "There is a hostile bandit. A fight seems unavoidable. Will you attack him?";
                    }
                    else if (enemy.ID == nameof(Golem))
                    {
                        info = "There is a dungeon boss. It’s a hostile golem. Will you attack him?";
                    }
                }
                else if (actor.TryGetComponent(out Treasure treasure))
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
                else if (actor.TryGetComponent(out Artifact artifact))
                {
                    if (artifact.IsEmpty)
                    {
                        info = "There is an altar, but nothing is placed on it.";
                    }
                    else
                    {
                        info = "Here is a legendary treasure. You can get it when you scavenge.";
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
                else if (actor.ID == nameof(TreasureChestKey))
                {
                    info = "There is a key to the treasure chest. You can get it when you scavenge.";
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

            // 所持していたアーティファクトが落ちた場合、ドアなどの上に重なることがある。
            foreach (Actor actor in cell.GetActors())
            {
                if (actor.ID == nameof(DroppedArtifact))
                {
                    info = "Here is a legendary treasure. You can get it when you scavenge.";
                }
            }

            // 探索回数に応じたタグを付与する。
            int count = _adventurer.ExploreRecord.Get(coords);
            if (count == 0)
            {
                return $"{info} (Unexplored)";
            }
            else if (count > 0)
            {
                return $"{info} (Explored: {count}) ";
            }
            else
            {
                Debug.LogWarning($"探索回数の値がマイナスになっている。: {count}");
                return info;
            }
        }
    }
}