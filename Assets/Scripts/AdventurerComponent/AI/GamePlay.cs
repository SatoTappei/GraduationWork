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

        // ���Ƀ��N�G�X�g����ہA���O��AI������������t���O�B
        bool _isPreInitialize;
        // FT�̃��O�����p�B���𐮗������ɖ`�����I�����ꍇ�Ɏ��B
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
             
            // ���ݒn�̖��́B
            format.CurrentLocation = DungeonManager.GetCell(_adventurer.Coords).Location.ToString();

            // �㉺���E�̃Z���ɉ������邩�A�T���ς݂Ȃ̂��𑗐M�B
            format.Surroundings.North = GetCellInfo(_adventurer.Coords + Vector2Int.up);
            format.Surroundings.South = GetCellInfo(_adventurer.Coords + Vector2Int.down);
            format.Surroundings.East = GetCellInfo(_adventurer.Coords + Vector2Int.right);
            format.Surroundings.West = GetCellInfo(_adventurer.Coords + Vector2Int.left);

            // �A�C�e���B��ނ��ƂɃ��X�g�ŊǗ����Ă���̂ŁA���ꂼ��̃��X�g�̐擪�̏���n���B
            List<string> items = new List<string>();
            foreach (IReadOnlyList<Item> e in _item.Get().Values)
            {
                items.Add($"{e[0].Name.English} (Usage: {e[0].Usage})");
            }
            if (items.Count == 0) items.Add("None");
            format.Items = items.ToArray();

            // �s�����O�B
            format.ActionLog = _adventurer.ActionLog.Log.ToArray();
            
            // ���B
            format.Information = _information.Information.Select(info => info.Text.English).ToArray();
            
            // ���̒�����1�s����I�ԁB
            format.AvailableActions = _actions.GetEntries().ToArray();
            
            // ���݂̃T�u�S�[���B
            format.Goal = _subGoalPath.GetCurrent().Description.English;

            // ��������v������Ă����ꍇ�B
            if (_isPreInitialize)
            {
                _isPreInitialize = false;
                Initialize();
            }
            
            // ��������Y��ČĂ΂ꂽ�ꍇ�B
            if (_ai == null)
            {
                Debug.LogWarning("�����������ɑ䎌�����N�G�X�g�����̂ŁA���N�G�X�g�O�ɏ����������B");
                Initialize();
            }

            string json = JsonUtility.ToJson(format, prettyPrint: true);
            string response = await _ai.RequestAsync(json, token);
            token.ThrowIfCancellationRequested();

            // �I�����ƃX�R�A���X�y�[�X��؂�ŕԂ��Ă��邱�Ƃ�z��B
            // �_�u���N�I�[�e�[�V�������t���Ă���ꍇ������B
            string result = response.Split()[0].Trim('"');

            // ���O�ɒǉ��B
            _logs.Add($"{json}\n{result}");
            if (result == "RequestHelp") _isHelped = true;

            return result;
        }

        void Initialize()
        {
            if (_adventurer.Sheet == null)
            {
                Debug.LogWarning("�`���҂̃f�[�^���ǂݍ��܂�Ă��Ȃ��B");

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
                // ���̍s����I���������R�A���ɕK�v�ȏ�񂪖������m�F����p�r�B
                $"- Select one of the AvailableActions.\n" +
                $"- The reason for choosing that action is also output.\n" +
                $"- Is there any other information you would like to know when choosing an action?";
#endif
                _ai = new AIClient(prompt);
            }
        }

        string GetCellInfo(Vector2Int coords)
        {
            // �ǂ̏ꍇ�B
            if (Blueprint.Base[coords.y][coords.x] == '#') return "Wall";

            // ���̍��W�ɉ������邩���������B
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
                        info = "There is a dungeon boss. It�fs a hostile golem. Will you attack him?";
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
                    // ���A�����A���̑��͂��̂܂�ID��Ԃ��B
                    info = actor.ID;
                }
            }

            // �������Ă����A�[�e�B�t�@�N�g���������ꍇ�A�h�A�Ȃǂ̏�ɏd�Ȃ邱�Ƃ�����B
            foreach (Actor actor in cell.GetActors())
            {
                if (actor.ID == nameof(DroppedArtifact))
                {
                    info = "Here is a legendary treasure. You can get it when you scavenge.";
                }
            }

            // �T���񐔂ɉ������^�O��t�^����B
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
                Debug.LogWarning($"�T���񐔂̒l���}�C�i�X�ɂȂ��Ă���B: {count}");
                return info;
            }
        }
    }
}