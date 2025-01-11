using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game
{
    public class PreActionEvaluator : MonoBehaviour
    {
        Adventurer _adventurer;
        SubGoalPath _subGoalPath;
        AvailableActions _actions;
        MadnessStatusEffect _madness;

        Artifact _artifact;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _subGoalPath = GetComponent<SubGoalPath>();
            _actions = GetComponent<AvailableActions>();
            _madness = GetComponent<MadnessStatusEffect>();
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
            IReadOnlyList<Actor> northActors = DungeonManager.GetActors(_adventurer.Coords + Vector2Int.up);
            IReadOnlyList<Actor> southActors = DungeonManager.GetActors(_adventurer.Coords + Vector2Int.down);
            IReadOnlyList<Actor> eastActors = DungeonManager.GetActors(_adventurer.Coords + Vector2Int.right);
            IReadOnlyList<Actor> westActors = DungeonManager.GetActors(_adventurer.Coords + Vector2Int.left);

            bool isAdventurerExistNorth = false;
            bool isAdventurerExistSouth = false;
            bool isAdventurerExistEast = false;
            bool isAdventurerExistWest = false;
            bool isEnemyExistNorth = false;
            bool isEnemyExistSouth = false;
            bool isEnemyExistEast = false;
            bool isEnemyExistWest = false;
            bool isScavengeableNorth = false;
            bool isScavengeableSouth = false;
            bool isScavengeableEast = false;
            bool isScavengeableWest = false;
            foreach (Actor a in northActors)
            {
                if (a is Adventurer) isAdventurerExistNorth = true;
                if (a is Enemy) isEnemyExistNorth = true;
                if (a is IScavengeable) isScavengeableNorth = true;
            }
            foreach (Actor a in southActors)
            {
                if (a is Adventurer) isAdventurerExistSouth = true;
                if (a is Enemy) isEnemyExistSouth = true;
                if (a is IScavengeable) isScavengeableSouth = true;
            }
            foreach (Actor a in eastActors)
            {
                if (a is Adventurer) isAdventurerExistEast = true;
                if (a is Enemy) isEnemyExistEast = true;
                if (a is IScavengeable) isScavengeableEast = true;
            }
            foreach (Actor a in westActors)
            {
                if (a is Adventurer) isAdventurerExistWest = true;
                if (a is Enemy) isEnemyExistWest = true;
                if (a is IScavengeable) isScavengeableWest = true;
            }

            // ���ꂼ��̕����ɖ`���҂�����ꍇ�A���̕����Ɉړ��ł��Ȃ��B
            // ���Ȃ��ꍇ�̃X�R�A�́A����M������ۂ̏�Ԃŋ��炸�Ɉړ�����悤�������߂�0��菭�������B
            _actions.SetScore("MoveNorth", isAdventurerExistNorth ? -1.0f : 0.1f);
            _actions.SetScore("MoveSouth", isAdventurerExistSouth ? -1.0f : 0.1f);
            _actions.SetScore("MoveEast", isAdventurerExistEast ? -1.0f : 0.1f);
            _actions.SetScore("MoveWest", isAdventurerExistWest ? -1.0f : 0.1f);

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

            // �u�����Ɉړ�����B�v�̑I�����B
            // ���݂̃T�u�S�[�����u�_���W�����̓����ɖ߂�B�v�̏ꍇ�A�ړ����X�R�A�������B
            // ����ȊO�̎��͑I��������O���B
            if (_subGoalPath.GetCurrent() == null)
            {
                _actions.SetScore("MoveToEntrance", -1.0f);
            }
            else if (_subGoalPath.GetCurrent().Description.Japanese == "�_���W�����̓����ɖ߂�B")
            {
                _actions.SetScore("MoveToEntrance", 0.5f);
            }
            else
            {
                _actions.SetScore("MoveToEntrance", -1.0f);
            }

            // �u�A�[�e�B�t�@�N�g�̈ʒu�Ɉړ�����B�v�̑I�����B
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
            if (isEnemyExistNorth || 
                isEnemyExistSouth || 
                isEnemyExistEast || 
                isEnemyExistWest)
            {
                _actions.SetScore("AttackToEnemy", 1.0f);
            }
            else
            {
                _actions.SetScore("AttackToEnemy", -1.0f);
            }

            // ���ꂩ�̕����ɖ`���҂�����ꍇ�́A���̖͂`���҂��U���������͉�b�ł���B
            if (isAdventurerExistNorth || 
                isAdventurerExistSouth || 
                isAdventurerExistEast || 
                isAdventurerExistWest)
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
            if(isScavengeableNorth || 
               isScavengeableSouth || 
               isScavengeableEast ||
               isScavengeableWest)
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
            int explore = _adventurer.Status.ExploreRecord.Get(_adventurer.Coords);
            if (explore >= 5)
            {
                _actions.SetScore("RequestHelp", explore / 10.0f);
            }
            else
            {
                _actions.SetScore("RequestHelp", -1.0f);
            }
        }
    }
}
