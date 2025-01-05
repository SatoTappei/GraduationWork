using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ActionEvaluator : MonoBehaviour
    {
        Adventurer _adventurer;
        SubGoalPath _subGoalPath;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _subGoalPath = GetComponent<SubGoalPath>();
        }

        public IEnumerable<(string, float)> Evaluate()
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
            yield return ("MoveNorth", isAdventurerExistNorth ? -1.0f : 0);
            yield return ("MoveSouth", isAdventurerExistSouth ? -1.0f : 0);
            yield return ("MoveEast", isAdventurerExistEast ? -1.0f : 0);
            yield return ("MoveWest", isAdventurerExistWest ? -1.0f : 0);

            // 壁判定。
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.up).IsImpassable())
            {
                yield return ("MoveNorth", -1.0f);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.down).IsImpassable())
            {
                yield return ("MoveSouth", -1.0f);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.right).IsImpassable())
            {
                yield return ("MoveEast", -1.0f);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.left).IsImpassable())
            {
                yield return ("MoveWest", -1.0f);
            }

            // 「入口に移動する。」の選択肢。
            // 現在のサブゴールが「ダンジョンの入口に戻る。」の場合、移動よりスコアが高い。
            // それ以外の時は選択肢から外す。
            if (_subGoalPath.GetCurrent() == null)
            {
                yield return ("MoveToEntrance", -1.0f);
            }
            else if (_subGoalPath.GetCurrent().Description.Japanese == "ダンジョンの入口に戻る。")
            {
                yield return ("MoveToEntrance", 0.5f);
            }
            else
            {
                yield return ("MoveToEntrance", -1.0f);
            }

            // 何れかの方向に敵がいる場合は、周囲の敵に攻撃できる。
            // スコアは移動より上。
            if (isEnemyExistNorth || 
                isEnemyExistSouth || 
                isEnemyExistEast || 
                isEnemyExistWest)
            {
                yield return ("AttackToEnemy", 0.5f);
            }
            else
            {
                yield return ("AttackToEnemy", -1.0f);
            }

            // 何れかの方向に冒険者がいる場合は、周囲の冒険者を攻撃もしくは会話できる。
            // スコアは移動より上。
            if (isAdventurerExistNorth || 
                isAdventurerExistSouth || 
                isAdventurerExistEast || 
                isAdventurerExistWest)
            {
                yield return ("AttackToAdventurer", 0.5f);
                yield return ("TalkWithAdventurer", 0.5f);
            }
            else
            {
                yield return ("AttackToAdventurer", -1.0f);
                yield return ("TalkWithAdventurer", -1.0f);
            }

            // 何れかの方向に宝箱やレバーなどインタラクトできる対象がある場合は、周囲を漁ることが出来る。
            // スコアは移動より上。
            if(isScavengeableNorth || 
               isScavengeableSouth || 
               isScavengeableEast ||
               isScavengeableWest)
            {
                yield return ("Scavenge", 0.5f);
            }
            else
            {
                yield return ("Scavenge", -1.0f);
            }
        }
    }
}
