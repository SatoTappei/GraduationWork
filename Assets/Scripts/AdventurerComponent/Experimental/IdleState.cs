using AI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Game.ItemData;

namespace Game.Experimental.FSM
{
    public class IdleState : State
    {
        [System.Serializable]
        class RequestFormat
        {
            public string Goal;
            public string CurrentLocation;
            public string[] Items;
            public string[] Information;
            public string[] Choices;
        }

        Adventurer _adventurer;
        HoldInformation _information;
        AvailableActions _actions;
        SubGoalPath _subGoalPath;
        ItemInventory _item;
        AIClient _ai;

        bool _isInitialized;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _information = GetComponent<HoldInformation>();
            _actions = GetComponent<AvailableActions>();
            _subGoalPath = GetComponent<SubGoalPath>();
            _item = GetComponent<ItemInventory>();
        }

        protected override async UniTask<string> EnterAsync(CancellationToken token)
        {
            // 最初に呼び出したタイミングでAIを初期化。
            if (!_isInitialized)
            {
                _isInitialized = true;
                Initialize();
            }

            await UniTask.Yield(cancellationToken: token);

            return "Idle";
        }

        protected override async UniTask<string> ExitAsync(CancellationToken token)
        {
            await UniTask.Yield(cancellationToken: token);

            return "Idle";
        }

        protected override async UniTask<string> StayAsync(CancellationToken token)
        {
            RequestFormat format = new RequestFormat();

            // 現在のサブゴール。
            format.Goal = _subGoalPath.GetCurrent().Description.English;

            // 現在地の名称。
            format.CurrentLocation = DungeonManager.GetCell(_adventurer.Coords).Location.ToString();

            // アイテム。種類ごとにリストで管理しているので、それぞれのリストの先頭の情報を渡す。
            List<string> items = new List<string>();
            foreach (IReadOnlyList<Item> e in _item.Get().Values)
            {
                items.Add($"{e[0].Name.English} (Usage: {e[0].Usage})");
            }
            if (items.Count == 0) items.Add("None");
            format.Items = items.ToArray();

            // 情報。
            format.Information = _information.Information.Select(info => info.Text.English).ToArray();

            // この中から1つ行動を選ぶ。
            Situation situation = new Situation();
            format.Choices = Choice.Get(situation).ToArray();

            string json = JsonUtility.ToJson(format, prettyPrint: true);
            string response = await _ai.RequestAsync(json, token);
            token.ThrowIfCancellationRequested();

            // 指定した出力形式に合っていない場合は整形する。
            string result = response.Trim('"');

            // 選んだ行動によって遷移先のステートが変わる。
            if (result == "ExploreThisRoom")
            {
                Transition(nameof(ExploreThisRoomState));
            }
            else if (result == "MoveForward")
            {
                Transition(nameof(MoveForwardState));
            }
            else
            {
                Debug.LogWarning($"遷移先が無い。: {result}");
                Transition(nameof(IdleState));
            }
            
            return "Idle";
        }

        void Initialize()
        {
            string prompt =
                $"# Instructions\n" +
                $"- Your character’s profiles are as follows.\n" +
                $"- Consider these profiles carefully when deciding the next action.\n" +
                $"- Avoid actions that go against their personality or expose their weaknesses.\n" +
                $"# CharacterProfiles(Japanese)\n" +
                $"- {_adventurer.Sheet.Personality}\n" +
                $"- {_adventurer.Sheet.Motivation}\n" +
                $"- {_adventurer.Sheet.Weaknesses}\n" +
                $"# OutputFormat\n" +
#if true
                $"- Select one of the Choices and output the value only.";
#else
                // その行動を選択した理由、他に必要な情報が無いか確認する用途。
                $"- Select one of the Choices.\n" +
                $"- The reason for choosing that action is also output.\n" +
                $"- Is there any other information you would like to know when choosing an action?";
#endif
            _ai = new AIClient(prompt);
        }
    }
}
