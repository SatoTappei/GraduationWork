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
            // アーティファクトの生成座標を手動で指定。
            _artifact = DungeonManager
                .GetActors(new Vector2Int(17, 21))
                .Select(a => a as Artifact)
                .FirstOrDefault();

            if (_artifact == null)
            {
                Debug.LogWarning("アーティファクトが見つからない。");
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

            // それぞれの方向に冒険者がいる場合、その方向に移動できない。
            // いない場合のスコアは、箱や樽が空っぽの状態で漁らずに移動するよう促すために0より少し高い。
            _actions.SetScore("MoveNorth", isAdventurerExistNorth ? -1.0f : 0.1f);
            _actions.SetScore("MoveSouth", isAdventurerExistSouth ? -1.0f : 0.1f);
            _actions.SetScore("MoveEast", isAdventurerExistEast ? -1.0f : 0.1f);
            _actions.SetScore("MoveWest", isAdventurerExistWest ? -1.0f : 0.1f);

            // 燃え上がっているタイルの場合はスコアを0にし、その方向に移動しないように促す。
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

            // 壁がある歩行には移動できない。
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

            // 「入口に移動する。」の選択肢。
            // 現在のサブゴールが「ダンジョンの入口に戻る。」の場合、移動よりスコアが高い。
            // それ以外の時は選択肢から外す。
            if (_subGoalPath.GetCurrent() == null)
            {
                _actions.SetScore("MoveToEntrance", -1.0f);
            }
            else if (_subGoalPath.GetCurrent().Description.Japanese == "ダンジョンの入口に戻る。")
            {
                _actions.SetScore("MoveToEntrance", 0.5f);
            }
            else
            {
                _actions.SetScore("MoveToEntrance", -1.0f);
            }

            // 「アーティファクトの位置に移動する。」の選択肢。
            // アーティファクトが出現している場合は最優先で向かうよう促す。
            if (_artifact.IsEmpty)
            {
                _actions.SetScore("MoveToArtifact", -1.0f);
            }
            else
            {
                // 大部屋にいる場合、アーティファクトがある座標の1歩手前の座標を目指す。
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

            // 何れかの方向に敵がいる場合は、周囲の敵に攻撃できる。
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

            // 何れかの方向に冒険者がいる場合は、周囲の冒険者を攻撃もしくは会話できる。
            if (isAdventurerExistNorth || 
                isAdventurerExistSouth || 
                isAdventurerExistEast || 
                isAdventurerExistWest)
            {
                // 狂気状態のときは攻撃しかできない。
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

            // 何れかの方向に宝箱やレバーなどインタラクトできる対象がある場合は、周囲を漁ることが出来る。
            if(isScavengeableNorth || 
               isScavengeableSouth || 
               isScavengeableEast ||
               isScavengeableWest)
            {
                // 狂気状態のときは漁ることが出来ない。
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

            // 同じセルを何回も通った場合、周囲をうろうろしている可能性がある。
            // 回数が多いほど助け(AIの初期化)を求めるよう強く促す。
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
