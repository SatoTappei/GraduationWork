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
            // �ŏ��ɌĂяo�����^�C�~���O��AI���������B
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

            // ���݂̃T�u�S�[���B
            format.Goal = _subGoalPath.GetCurrent().Description.English;

            // ���ݒn�̖��́B
            format.CurrentLocation = DungeonManager.GetCell(_adventurer.Coords).Location.ToString();

            // �A�C�e���B��ނ��ƂɃ��X�g�ŊǗ����Ă���̂ŁA���ꂼ��̃��X�g�̐擪�̏���n���B
            List<string> items = new List<string>();
            foreach (IReadOnlyList<Item> e in _item.Get().Values)
            {
                items.Add($"{e[0].Name.English} (Usage: {e[0].Usage})");
            }
            if (items.Count == 0) items.Add("None");
            format.Items = items.ToArray();

            // ���B
            format.Information = _information.Information.Select(info => info.Text.English).ToArray();

            // ���̒�����1�s����I�ԁB
            Situation situation = new Situation();
            format.Choices = Choice.Get(situation).ToArray();

            string json = JsonUtility.ToJson(format, prettyPrint: true);
            string response = await _ai.RequestAsync(json, token);
            token.ThrowIfCancellationRequested();

            // �w�肵���o�͌`���ɍ����Ă��Ȃ��ꍇ�͐��`����B
            string result = response.Trim('"');

            // �I�񂾍s���ɂ���đJ�ڐ�̃X�e�[�g���ς��B
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
                Debug.LogWarning($"�J�ڐ悪�����B: {result}");
                Transition(nameof(IdleState));
            }
            
            return "Idle";
        }

        void Initialize()
        {
            string prompt =
                $"# Instructions\n" +
                $"- Your character�fs profiles are as follows.\n" +
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
                // ���̍s����I���������R�A���ɕK�v�ȏ�񂪖������m�F����p�r�B
                $"- Select one of the Choices.\n" +
                $"- The reason for choosing that action is also output.\n" +
                $"- Is there any other information you would like to know when choosing an action?";
#endif
            _ai = new AIClient(prompt);
        }
    }
}
