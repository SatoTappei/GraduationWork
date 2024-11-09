using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DungeonManager : MonoBehaviour
    {
        PlacedActors _placedActors;
        Dungeon _dungeon;
        TerrainFeatures _terrainFeatures;
        AStar _aStar;

        void Awake()
        {
            _placedActors = new PlacedActors();
            _dungeon = GetComponent<Dungeon>();
            _terrainFeatures = GetComponent<TerrainFeatures>();
            _aStar = new AStar(_dungeon.Grid);
        }

        public static DungeonManager Find()
        {
            return GameObject.FindGameObjectWithTag("DungeonManager").GetComponent<DungeonManager>();
        }

        public void AddActorOnCell(Vector2Int coords, Actor actor)
        {
            GetCell(coords).AddActor(actor);
            _placedActors.Add(actor);
        }

        public void RemoveActorOnCell(Vector2Int coords, Actor actor)
        {
            GetCell(coords).RemoveActor(actor);
            _placedActors.Remove(actor);
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
            foreach (Actor a in _placedActors[id])
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
            return _dungeon.Grid[coords.y, coords.x];
        }

        public bool Pathfinding(int startX, int startY, int goalX, int goalY, List<Cell> result)
        {
            Vector2Int start = new Vector2Int(startX, startY);
            Vector2Int goal = new Vector2Int(goalX, goalY);
            return Pathfinding(start, goal, result);
        }

        public bool Pathfinding(Vector2Int start, Vector2Int goal, List<Cell> result)
        {
            return _aStar.Pathfinding(start, goal, result);
        }

        public bool TryGetTerrainFeature(Vector2Int coords, out SharedInformation feature)
        {
            return _terrainFeatures.TryGetInformation(coords, out feature);
        }
    }
}