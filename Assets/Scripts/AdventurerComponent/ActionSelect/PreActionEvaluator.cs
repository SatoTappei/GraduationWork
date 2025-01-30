using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.ItemData;

namespace Game
{
    public class PreActionEvaluator : MonoBehaviour
    {
        struct State
        {
            public bool Adventurer;
            public bool Enemy;
            public bool Scavengeable;
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

            // ���ꂼ��̕����ɖ`���҂�����ꍇ�A���̕����Ɉړ��ł��Ȃ��B
            // ���Ȃ��ꍇ�̃X�R�A�́A����M������ۂ̏�Ԃŋ��炸�Ɉړ�����悤�������߂�0��菭�������B
            _actions.SetScore("MoveNorth", north.Adventurer ? -1.0f : 0.1f);
            _actions.SetScore("MoveSouth", south.Adventurer ? -1.0f : 0.1f);
            _actions.SetScore("MoveEast", east.Adventurer ? -1.0f : 0.1f);
            _actions.SetScore("MoveWest", west.Adventurer ? -1.0f : 0.1f);

            // �R���オ���Ă���^�C���̏ꍇ�̓X�R�A��0�ɂ��A���̕����Ɉړ����Ȃ��悤�ɑ����B
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.up).TerrainEffect == TerrainEffect.Flaming)
            {
                _actions.SetScore("MoveNorth", 0);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.down).TerrainEffect == TerrainEffect.Flaming)
            {
                _actions.SetScore("MoveSouth", 0);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.right).TerrainEffect == TerrainEffect.Flaming)
            {
                _actions.SetScore("MoveEast", 0);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.left).TerrainEffect == TerrainEffect.Flaming)
            {
                _actions.SetScore("MoveWest", 0);
            }

            // �ǂ�������s�ɂ͈ړ��ł��Ȃ��B
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.up).IsImpassable())
            {
                _actions.SetScore("MoveNorth", -1.0f);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.down).IsImpassable())
            {
                _actions.SetScore("MoveSouth", -1.0f);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.right).IsImpassable())
            {
                _actions.SetScore("MoveEast", -1.0f);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.left).IsImpassable())
            {
                _actions.SetScore("MoveWest", -1.0f);
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
                // �啔���ɂ���ꍇ�A�A�[�e�B�t�@�N�g��������W��1����O�̍��W��ڎw���B
                bool isBossRoom = Blueprint.Location[_adventurer.Coords.y][_adventurer.Coords.x] == '5';
                bool isAway = _adventurer.Coords != new Vector2Int(17, 20);
                if (isBossRoom && isAway)
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
            int explore = _adventurer.ExploreRecord.Get(_adventurer.Coords);
            if (explore >= 5)
            {
                _actions.SetScore("RequestHelp", explore / 10.0f);
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
            State direction = new State();
            foreach (Actor a in DungeonManager.GetActors(coords))
            {
                if (a is Adventurer) direction.Adventurer = true;
                if (a is Enemy) direction.Enemy = true;
                if (a is IScavengeable) direction.Scavengeable = true;
            }

            return direction;
        }
    }
}
