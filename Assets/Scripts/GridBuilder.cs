using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class GridBuilder
    {
        public static Cell[,] Create(Vector3 basePosition)
        {
            int h = Blueprint.Height;
            int w = Blueprint.Width;

            Cell[,] grid = new Cell[h, w];
            for (int i = 0; i < h; i++)
            {
                for (int k = 0; k < w; k++)
                {
                    grid[i, k] = GetNewCell(basePosition, k, i);
                }
            }

            return grid;
        }

        static Cell GetNewCell(Vector3 basePosition, int x, int y)
        {
            Vector3 position = basePosition + new Vector3(x, 0, y);
            int cost = GetCellCost(x, y);
            Terrain terrain = GetCellTerrain(x, y);
            return new Cell(position, x, y, cost, terrain);
        }

        static int GetCellCost(int x, int y)
        {
            // 現状コストの概念が無い。
            return 1;
        }

        static Terrain GetCellTerrain(int x, int y)
        {
            if (Blueprint.Base[y][x] == '#') return Terrain.Wall;

            // 宝箱は上下左右の向きがあるので4種類の文字で判定。
            bool isTreasure = "2468".Contains(Blueprint.Treasures[y][x]);
            if (isTreasure) return Terrain.Impassable;

            // 一応"通行可能"の地形を作ったが、特殊なルールが必要になるまでは扉や敵の湧き地点も床扱い。
            // 座標をそれぞれのBlueprintで判定する必要があるので面倒くさい。
            return Terrain.Floor;
        }
    }
}
