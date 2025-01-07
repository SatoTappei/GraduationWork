using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PreActionEvaluator : MonoBehaviour
    {
        Adventurer _adventurer;
        SubGoalPath _subGoalPath;
        AvailableActions _actions;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _subGoalPath = GetComponent<SubGoalPath>();
            _actions = GetComponent<AvailableActions>();
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
            // ����ꍇ�̃X�R�A�́A����M������ۂ̏�Ԃŋ��炸�Ɉړ�����悤�������߂�0��菭�������B
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

            // ���ꂩ�̕����ɓG������ꍇ�́A���͂̓G�ɍU���ł���B
            // �X�R�A�͈ړ�����B
            if (isEnemyExistNorth || 
                isEnemyExistSouth || 
                isEnemyExistEast || 
                isEnemyExistWest)
            {
                _actions.SetScore("AttackToEnemy", 0.5f);
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
                _actions.SetScore("AttackToAdventurer", 0);
                _actions.SetScore("TalkWithAdventurer", 0.5f);
            }
            else
            {
                _actions.SetScore("AttackToAdventurer", -1.0f);
                _actions.SetScore("TalkWithAdventurer", -1.0f);
            }

            // ���ꂩ�̕����ɕ󔠂⃌�o�[�ȂǃC���^���N�g�ł���Ώۂ�����ꍇ�́A���͂����邱�Ƃ��o����B
            // �X�R�A�͈ړ�����B
            if(isScavengeableNorth || 
               isScavengeableSouth || 
               isScavengeableEast ||
               isScavengeableWest)
            {
                _actions.SetScore("Scavenge", 0.5f);
            }
            else
            {
                _actions.SetScore("Scavenge", -1.0f);
            }
        }
    }
}
