using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExploreRecord
    {
        int[,] _record;

        public ExploreRecord()
        {
            _record = new int[Blueprint.Height, Blueprint.Width];
        }

        public void Increase(Vector2Int coords)
        {
            _record[coords.y, coords.x]++;
        }

        public int Get(Vector2Int coords)
        {
            return _record[coords.y, coords.x];
        }

        public void Delete()
        {
            for (int i = 0; i < _record.GetLength(0); i++)
            {
                for (int k = 0; k < _record.GetLength(1); k++)
                {
                    _record[i, k] = 0;
                }
            }
        }
    }
}
