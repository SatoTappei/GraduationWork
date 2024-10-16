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
            // ����R�X�g�̊T�O�������B
            return 1;
        }

        static Terrain GetCellTerrain(int x, int y)
        {
            if (Blueprint.Base[y][x] == '#') return Terrain.Wall;

            // �󔠂͏㉺���E�̌���������̂�4��ނ̕����Ŕ���B
            bool isTreasure = "2468".Contains(Blueprint.Treasures[y][x]);
            if (isTreasure) return Terrain.Impassable;

            // �ꉞ"�ʍs�\"�̒n�`����������A����ȃ��[�����K�v�ɂȂ�܂ł͔���G�̗N���n�_���������B
            // ���W�����ꂼ���Blueprint�Ŕ��肷��K�v������̂Ŗʓ|�������B
            return Terrain.Floor;
        }
    }
}
