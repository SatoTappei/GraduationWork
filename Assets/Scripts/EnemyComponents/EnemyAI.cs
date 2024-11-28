using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EnemyAI : MonoBehaviour
    {
        EnemyBlackboard _blackboard;
        DungeonManager _dungeonManager;

        void Awake()
        {
            _blackboard = GetComponent<EnemyBlackboard>();
            _dungeonManager = DungeonManager.Find();
        }

        public string ChoiceNextAction()
        {
            // 自身の上下左右に冒険者がいれば攻撃する。
            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    if (i == 0 && k == 0) continue;
                    if (i != 0 && k != 0) continue;

                    Cell cell = _dungeonManager.GetCell(_blackboard.Coords + new Vector2Int(k, i));
                    foreach (Actor actor in cell.GetActors())
                    {
                        if (actor is Adventurer) return "Attack Surrounding";
                    }
                }
            }

            List<Vector2Int> candidate = new List<Vector2Int>();

            // 移動先の候補となる座標。生成された座標の周囲8つのセルまでが移動範囲。
            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    int nx = _blackboard.SpawnCoords.x + k;
                    int ny = _blackboard.SpawnCoords.y + i;

                    if (Blueprint.Base[ny][nx] == '_')
                    {
                        candidate.Add(new Vector2Int(nx, ny));
                    }
                }
            }

            // 候補となる座標のうち、通行不可能もしくは斜め方向の座標を除く。
            for (int i = candidate.Count - 1; i >= 0; i--)
            {
                Cell cell = _dungeonManager.GetCell(candidate[i]);
                if (cell.IsImpassable()) candidate.RemoveAt(i);
                else if (1.4f <= Vector2Int.Distance(_blackboard.Coords, candidate[i])) candidate.RemoveAt(i);
            }

            // 残った座標の中からランダムに選び、その座標へ移動する方向を返す。
            Vector2Int choice = candidate[Random.Range(0, candidate.Count)];
            Vector2Int direction = choice - _blackboard.Coords;          
            if (direction == Vector2Int.zero) return "Idle";
            if (direction == Vector2Int.up) return "Move North";
            if (direction == Vector2Int.down) return "Move South";
            if (direction == Vector2Int.right) return "Move East";
            if (direction == Vector2Int.left) return "Move West";

            Debug.LogError("敵AIが正しく行動を選択出来ていない。");

            return string.Empty;
        }
    }
}
