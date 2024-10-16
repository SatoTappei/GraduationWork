using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AStar
    {
        Cell[,] _grid;
        List<Cell> _open;
        HashSet<Cell> _closed;

        public AStar(Cell[,] grid)
        {
            _grid = grid;
            _open = new List<Cell>();
            _closed = new HashSet<Cell>();
        }

        public void Pathfinding(Vector2Int startCoords, Vector2Int goalCoords, List<Cell> result)
        {
            if (!IsInGrid(_grid, startCoords.x, startCoords.y))
            {
                Debug.LogWarning($"経路探索のスタート座標がグリッド外: {startCoords}");
                return;
            }
            if (!IsInGrid(_grid, goalCoords.x, goalCoords.y))
            {
                Debug.LogWarning($"経路探索のゴール座標がグリッド外: {goalCoords}");
                return;
            }

            Cell start = _grid[startCoords.y, startCoords.x];
            Cell goal = _grid[goalCoords.y, goalCoords.x];

            _open.Clear();
            _open.Add(start);
            _closed.Clear();

            if (startCoords == goalCoords) return;

            while (_open.Count > 0)
            {
                Cell current = GetMinCostCell(_open);
                
                if (IsGoalCell(current, goal)) 
                { 
                    CreatePath(start, goal, result);
                    return; 
                }

                _open.Remove(current);
                _closed.Add(current);

                foreach (Cell neighbour in GetNeighbourCells(_grid, current.Coords))
                {
                    if (neighbour.IsImpassable()) continue;
                    if (_closed.Contains(neighbour)) continue;

                    int newGCost = current.GCost + Distance(current.Coords, neighbour.Coords);
                    int newHCost = Distance(neighbour.Coords, goalCoords);
                    bool isUnopened = !_open.Contains(neighbour);
                    if ((newGCost + newHCost < neighbour.FCost) || isUnopened)
                    {
                        neighbour.GCost = newGCost;
                        neighbour.HCost = newHCost;
                        neighbour.Parent = current;
                    }

                    if (isUnopened) _open.Add(neighbour);
                }
            }
        }

        static Cell GetMinCostCell(IReadOnlyList<Cell> list)
        {
            if (list.Count == 0) return null;

            Cell min = list[0];
            foreach (Cell c in list)
            {
                // 総コストが等しい場合、ゴールまでの直線距離が近い方(推定コスト)を選択。
                if (c.FCost < min.FCost || (c.FCost == min.FCost && c.HCost < min.HCost))
                {
                    min = c;
                }
            }

            return min;
        }

        static bool IsGoalCell(Cell current, Cell goal)
        {
            return current.Coords == goal.Coords;
        }

        static IEnumerable<Cell> GetNeighbourCells(Cell[,] grid, Vector2Int coords)
        {
            foreach (Vector2Int dir in GetDirection())
            {
                int x = coords.x + dir.x;
                int y = coords.y + dir.y;

                if (IsInGrid(grid, x, y)) yield return grid[y, x];
            }

            IEnumerable<Vector2Int> GetDirection()
            {
                yield return Vector2Int.up;
                yield return Vector2Int.down;
                yield return Vector2Int.left;
                yield return Vector2Int.right;
            }
        }

        static bool IsInGrid(Cell[,] grid, int x, int y)
        {
            int h = grid.GetLength(0);
            int w = grid.GetLength(1);
            return 0 <= y && y < h && 0 <= x && x < w;
        }

        static void CreatePath(Cell start, Cell goal, List<Cell> result)
        {
            if (result.Count > 0) result.Clear();

            Cell current = goal;
            while (current != start)
            {
                result.Add(current);
                current = current.Parent;
            }

            result.Reverse();
        }

        static int Distance(Vector2Int a, Vector2Int b)
        {
            int x = Mathf.Abs(a.x - b.x);
            int y = Mathf.Abs(a.y - b.y);

            if (x > y) return 14 * y + 10 * (x - y);
            else return 14 * x + 10 * (y - x);
        }

#if false // デバッグ用
        static void CheckCellStatus(Cell[,] grid)
        {
            foreach (Cell c in grid)
            {
                Debug.Log($"実コスト:{c.GCost} 推定コスト:{c.HCost} 親:{c.Parent}");
            }
        }
#endif
    }
}
