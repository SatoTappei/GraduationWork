using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using AI;

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
        AvailableActions _availableActions;
        SubGoalPath _subGoalPath;
        AIClient _ai;

        // 次にリクエストする際、事前にAIを初期化するフラグ。
        bool _isPreInitialize;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _information = GetComponent<HoldInformation>();
            _availableActions = GetComponent<AvailableActions>();
            _subGoalPath = GetComponent<SubGoalPath>();
        }

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
            
            // 行動ログ。
            format.ActionLog = _adventurer.Status.ActionLog.Log.ToArray();

            // 情報。ユーザーが送信したコメントの場合、英語ではなく日本語の文章が送信される。
            format.Information = _information.Information.Select(info => info.Text.English).ToArray();
            
            // この中から1つ行動を選ぶ。
            format.AvailableActions = _availableActions.GetEntries().ToArray();
            
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

            string response = await _ai.RequestAsync(JsonUtility.ToJson(format), token);
            token.ThrowIfCancellationRequested();

            // 選択肢とスコアがスペース区切りで返ってくることを想定。
            // ダブルクオーテーションが付いている場合もある。
            string result = response.Split()[0].Trim('"');
            
            return result;
        }

        void Initialize()
        {
            if (_adventurer.AdventurerSheet == null)
            {
                Debug.LogWarning("冒険者のデータが読み込まれていない。");

                _ai = new AIClient($"Select one of the AvailableActions and output the value only.");
            }
            else
            {
                string prompt =
                    $"# Instructions\n" +
                    $"- Your character’s profiles are as follows.\n" +
                    $"- Consider these profiles carefully when deciding the next action.\n" +
                    $"- Avoid actions that go against their personality or expose their weaknesses.\n" +
                    $"# CharacterProfiles(Japanese)\n" +
                    $"- {_adventurer.AdventurerSheet.Personality}\n" +
                    $"- {_adventurer.AdventurerSheet.Motivation}\n" +
                    $"- {_adventurer.AdventurerSheet.Weaknesses}\n" +
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

            // 探索回数に応じたタグを付与する。
            int count = _adventurer.Status.ExploreRecord.Get(coords);
            if (count == 0)
            {
                return $"[Unexplored] {info}";
            }
            else if (count == 1)
            {
                return $"[Exproled {count} time] {info}";
            }
            else if (count > 1)
            {
                return $"[Exproled {count} times] {info}";
            }
            else
            {
                Debug.LogWarning($"探索回数の値がマイナスになっている。: {count}");
                return info;
            }
        }
    }
}