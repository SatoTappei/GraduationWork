using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DungeonManager : MonoBehaviour
    {
        enum GizmosMode
        {
            Dungeon,
            TerrainFeatures,
        }

        [SerializeField] GizmosMode _gizmosMode;

        Dungeon _dungeon;
        TerrainFeatures _terrainFeatures;
        AStar _aStar;
        PlacedActors _placedActors;

        Dungeon Dungeon
        {
            get
            {
                if (_dungeon == null) _dungeon = GetComponent<Dungeon>();
                return _dungeon;
            }
        }

        AStar AStar
        {
            get
            {
                if (_aStar == null) _aStar = new AStar(Dungeon.Grid);
                return _aStar;
            }
        }

        PlacedActors PlacedActors
        {
            get
            {
                if (_placedActors == null) _placedActors = new PlacedActors();
                return _placedActors;
            }
        }

        public TerrainFeatures TerrainFeatures
        {
            get
            {
                if (_terrainFeatures == null) _terrainFeatures = GetComponent<TerrainFeatures>();
                return _terrainFeatures;
            }
        }

        void OnDrawGizmosSelected()
        {
            if (_gizmosMode == GizmosMode.Dungeon) Dungeon.Draw();
            if (_gizmosMode == GizmosMode.TerrainFeatures) TerrainFeatures.Draw();
        }

        public static DungeonManager Find()
        {
            return GameObject.FindGameObjectWithTag("DungeonManager").GetComponent<DungeonManager>();
        }

        public void AddActorOnCell(Vector2Int coords, Actor actor)
        {
            GetCell(coords).AddActor(actor);
            PlacedActors.Add(actor);
        }

        public void RemoveActorOnCell(Vector2Int coords, Actor actor)
        {
            GetCell(coords).RemoveActor(actor);
            PlacedActors.Remove(actor);
        }

        public void AddAvoidCell(Vector2Int coords)
        {
            GetCell(coords).IsAvoid = true;
        }

        public void RemoveAvoidCell(Vector2Int coords)
        {
            GetCell(coords).IsAvoid = false;
        }

        public IReadOnlyList<Actor> GetActorsOnCell(Vector2Int coords)
        {
            return GetCell(coords).GetActors();
        }

        public IEnumerable<Cell> GetCells(string id)
        {
            foreach (Actor a in PlacedActors[id])
            {
                yield return GetCell(a.Coords);
            }
        }

        public Cell GetCell(int x, int y)
        {
            Vector2Int coords = new Vector2Int(x, y);
            return GetCell(coords);
        }

        public Cell GetCell(Vector2Int coords)
        {
            return Dungeon.Grid[coords.y, coords.x];
        }

        public bool Pathfinding(int startX, int startY, int goalX, int goalY, List<Cell> result)
        {
            Vector2Int start = new Vector2Int(startX, startY);
            Vector2Int goal = new Vector2Int(goalX, goalY);
            return Pathfinding(start, goal, result);
        }

        public bool Pathfinding(Vector2Int start, Vector2Int goal, List<Cell> result)
        {
            return AStar.Pathfinding(start, goal, result);
        }
    }
}