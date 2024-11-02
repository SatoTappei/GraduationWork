using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IReadOnlyExploreRecord
    {
        public int GetCount(Vector2Int coords);
    }

    public class ExploreRecord : IReadOnlyExploreRecord
    {
        int[,] _count;

        public ExploreRecord(int height, int width)
        {
            _count = new int[height, width];
        }

        public void IncreaseCount(Vector2Int coords)
        {
            _count[coords.y, coords.x]++;
        }

        public int GetCount(Vector2Int coords)
        {
            return _count[coords.y, coords.x];
        }
    }
}
