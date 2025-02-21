using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.ItemData;
using System;

namespace Game
{
    public class PreActionEvaluator : MonoBehaviour
    {
        struct State
        {
            public bool Adventurer;
            public bool Enemy;
            public bool Scavengeable;
            public bool Flaming;
            public bool Impassable;
        }

        Adventurer _adventurer;
        SubGoalPath _subGoalPath;
        AvailableActions _actions;
        MadnessStatusEffect _madness;
        ItemInventory _item;

        Artifact _artifact;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _subGoalPath = GetComponent<SubGoalPath>();
            _actions = GetComponent<AvailableActions>();
            _madness = GetComponent<MadnessStatusEffect>();
            _item = GetComponent<ItemInventory>();
        }

        void Start()
        {
            // �A�[�e�B�t�@�N�g�̐������W���蓮�Ŏw��B
            _artifact = DungeonManager
                .GetActors(new Vector2Int(17, 21))
                .Select(a => a as Artifact)
                .FirstOrDefault();

            if (_artifact == null)
            {
                Debug.LogWarning("�A�[�e�B�t�@�N�g��������Ȃ��B");
            }
        }

        public void Evaluate()
        {
            State north = CheckCell(_adventurer.Coords + Vector2Int.up);
            State south = CheckCell(_adventurer.Coords + Vector2Int.down);
            State east = CheckCell(_adventurer.Coords + Vector2Int.right);
            State west = CheckCell(_adventurer.Coords + Vector2Int.left);

            // �ȉ��̗��R�ɂ��A0���傫���l��ݒ肵�Ă����B
            // �����R���オ���Ă���ꍇ�ȂǁA�ړ��͏o���邪���Ȃ������ǂ��ꍇ�����邽�߁B
            // ����M������ۂ̏�Ԃŋ��炸�Ɉړ�����悤�������߁B
            _actions.SetScore("MoveNorth", 0.1f);
            _actions.SetScore("MoveSouth", 0.1f);
            _actions.SetScore("MoveEast", 0.1f);
            _actions.SetScore("MoveWest", 0.1f);

            // �R���オ���Ă���^�C���̏ꍇ�̓X�R�A��0�ɂ��A���̕����Ɉړ����Ȃ��悤�ɑ����B
            if (north.Flaming) _actions.SetScore("MoveNorth", 0);
            if (south.Flaming) _actions.SetScore("MoveSouth", 0);
            if (east.Flaming) _actions.SetScore("MoveEast", 0);
            if (west.Flaming) _actions.SetScore("MoveWest", 0);

            // ���ꂼ��̕����ɖ`���҂�����ꍇ�A���̕����Ɉړ��ł��Ȃ��B
            if (north.Adventurer) _actions.SetScore("MoveNorth", -1.0f);
            if (south.Adventurer) _actions.SetScore("MoveSouth", -1.0f);
            if (east.Adventurer) _actions.SetScore("MoveEast", -1.0f);
            if (west.Adventurer) _actions.SetScore("MoveWest", -1.0f);

            // �ǂ�������s�ɂ͈ړ��ł��Ȃ��B
            if (north.Impassable) _actions.SetScore("MoveNorth", -1.0f);
            if (south.Impassable) _actions.SetScore("MoveSouth", -1.0f);
            if (east.Impassable) _actions.SetScore("MoveEast", -1.0f);
            if (west.Impassable) _actions.SetScore("MoveWest", -1.0f);

            // �X�R�A��0���傫�������ւ͉��̏�Q�������ړ��ł���B
            // ���̒�����T���񐔂����Ȃ������Ɉړ�����悤�����B
            // �Ώۂ�����̂ɃX���[���Ȃ��悤�A�U���⋙��Ȃǂ̍s����肩�͒Ⴂ�l�ɐݒ肵�Ă����B
            List<(string, int)> explore = new List<(string, int)>(4);
            if (_actions.GetScore("MoveNorth") > 0)
            {
                int count = _adventurer.ExploreRecord.Get(_adventurer.Coords + Vector2Int.up);
                explore.Add(("North", count));
            }
            if(_actions.GetScore("MoveSouth") > 0)
            {
                int count = _adventurer.ExploreRecord.Get(_adventurer.Coords + Vector2Int.down);
                explore.Add(("South", count));
            }
            if (_actions.GetScore("MoveEast") > 0)
            {
                int count = _adventurer.ExploreRecord.Get(_adventurer.Coords + Vector2Int.down);
                explore.Add(("East", count));
            }
            if (_actions.GetScore("MoveWest") > 0)
            {
                int count = _adventurer.ExploreRecord.Get(_adventurer.Coords + Vector2Int.down);
                explore.Add(("West", count));
            }
            if (explore.Count > 0)
            {
                string minExplore =
                    explore
                    .OrderBy(x => Guid.NewGuid())
                    .OrderBy(x => x.Item2)
                    .First()
                    .Item1;
                _actions.SetScore($"Move{minExplore}", 0.33f);
            }

            // �u�����Ɉړ�����v�̑I�����B
            // ���݂̃T�u�S�[�����u�_���W�����̓����ɖ߂�v�̏ꍇ�A�ړ����X�R�A�������B
            // ����ȊO�̎��͑I��������O���B
            if (_subGoalPath.GetCurrent() == null)
            {
                _actions.SetScore("MoveToEntrance", -1.0f);
            }
            else if (_subGoalPath.GetCurrent().Description.Japanese == "�_���W�����̓����ɖ߂�")
            {
                _actions.SetScore("MoveToEntrance", 0.5f);
            }
            else
            {
                _actions.SetScore("MoveToEntrance", -1.0f);
            }

            // �u�A�[�e�B�t�@�N�g�̈ʒu�Ɉړ�����v�̑I�����B
            // �A�[�e�B�t�@�N�g���o�����Ă���ꍇ�͍ŗD��Ō������悤�����B
            if (_artifact.IsEmpty)
            {
                _actions.SetScore("MoveToArtifact", -1.0f);
            }
            else
            {
                // �A�[�e�B�t�@�N�g��������W��1����O�̍��W��ڎw���B
                if (_adventurer.Coords != new Vector2Int(17, 20))
                {
                    _actions.SetScore("MoveToArtifact", 1.0f);
                }
                else
                {
                    _actions.SetScore("MoveToArtifact", -1.0f);
                }
            }

            // ���ꂩ�̕����ɓG������ꍇ�́A���͂̓G�ɍU���ł���B
            if (north.Enemy || south.Enemy || east.Enemy || west.Enemy)
            {
                _actions.SetScore("AttackToEnemy", 1.0f);
            }
            else
            {
                _actions.SetScore("AttackToEnemy", -1.0f);
            }

            // ���ꂩ�̕����ɖ`���҂�����ꍇ�́A���̖͂`���҂��U���������͉�b�ł���B
            if (north.Adventurer || south.Adventurer || east.Adventurer || west.Adventurer)
            {
                // ���C��Ԃ̂Ƃ��͍U�������ł��Ȃ��B
                if (_madness.IsValid)
                {
                    _actions.SetScore("AttackToAdventurer", 1.0f);
                    _actions.SetScore("TalkWithAdventurer", -1.0f);
                }
                else
                {
                    _actions.SetScore("AttackToAdventurer", 0);
                    _actions.SetScore("TalkWithAdventurer", 0.5f);
                }
            }
            else
            {
                _actions.SetScore("AttackToAdventurer", -1.0f);
                _actions.SetScore("TalkWithAdventurer", -1.0f);
            }

            // ���ꂩ�̕����ɕ󔠂⃌�o�[�ȂǃC���^���N�g�ł���Ώۂ�����ꍇ�́A���͂����邱�Ƃ��o����B
            if(north.Scavengeable || south.Scavengeable || east.Scavengeable || west.Scavengeable)
            {
                // ���C��Ԃ̂Ƃ��͋��邱�Ƃ��o���Ȃ��B
                if (_madness.IsValid)
                {
                    _actions.SetScore("Scavenge", -1.0f);
                }
                else
                {
                    _actions.SetScore("Scavenge", 0.5f);
                }
            }
            else
            {
                _actions.SetScore("Scavenge", -1.0f);
            }

            // �����Z����������ʂ����ꍇ�A���͂����낤�낵�Ă���\��������B
            // �񐔂������قǏ���(AI�̏�����)�����߂�悤���������B
            int history = _adventurer.ExploreRecord.Get(_adventurer.Coords);
            if (history >= 5)
            {
                _actions.SetScore("RequestHelp", history / 10.0f);
            }
            else
            {
                _actions.SetScore("RequestHelp", -1.0f);
            }

            _actions.SetScore("ThrowItem", -1.0f);

            // �����邱�Ƃ��o����A�C�e���������Ă���ꍇ�̂݁u�A�C�e���𓊂���v�I������I�Ԃ��Ƃ��o����B
            foreach (IReadOnlyList<Item> items in _item.Get().Values)
            {
                if (items[0].Usage == Usage.Throw)
                {
                    // �D��x�͍Œ�ŗǂ��H
                    _actions.SetScore("ThrowItem", 0);
                    break;
                }
            }
        }

        static State CheckCell(Vector2Int coords)
        {
            State state = new State();
            foreach (Actor a in DungeonManager.GetActors(coords))
            {
                if (a is Adventurer) state.Adventurer = true;
                if (a is Enemy) state.Enemy = true;
                if (a is IScavengeable) state.Scavengeable = true;
            }

            state.Flaming = DungeonManager.GetCell(coords).TerrainEffect == TerrainEffect.Flaming;
            state.Impassable = DungeonManager.GetCell(coords).IsImpassable();

            return state;
        }
    }
}
