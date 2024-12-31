using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Pathfinding : MonoBehaviour
    {
        AStar _aStar;

        void Start()
        {
            Initialize();
        }

        public static bool TryFind(out Pathfinding result)
        {
            result = GameObject.FindGameObjectWithTag("DungeonManager").GetComponent<Pathfinding>();
            return result != null;
        }

        public bool CalculatePath(Vector2Int start, Vector2Int goal, List<Cell> result)
        {
            if (_aStar == null) Initialize();

            return _aStar.Pathfinding(start, goal, result);
        }

        void Initialize()
        {
            Dungeon dungeon = GetComponent<Dungeon>();
            _aStar = new AStar(dungeon.Grid);
        }
    }
}