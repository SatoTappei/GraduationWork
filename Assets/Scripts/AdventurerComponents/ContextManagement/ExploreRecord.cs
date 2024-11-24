using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExploreRecord : MonoBehaviour
    {
        int[,] _count;

        void Awake()
        {
            _count = new int[Blueprint.Height, Blueprint.Width];
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
