using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DungeonManager : MonoBehaviour
    {
        Dungeon _dungeon;
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

        void OnDrawGizmosSelected()
        {
            Dungeon.Draw();
        }

        public static DungeonManager Find()
        {
            return FindAnyObjectByType<DungeonManager>();
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

        public Cell GetCell(Vector2Int coords)
        {
            return Dungeon.Grid[coords.y, coords.x];
        }

        public void Pathfinding(int startX, int startY, int goalX, int goalY, List<Cell> result)
        {
            Vector2Int start = new Vector2Int(startX, startY);
            Vector2Int goal = new Vector2Int(goalX, goalY);
            Pathfinding(start, goal, result);
        }

        public void Pathfinding(Vector2Int start, Vector2Int goal, List<Cell> result)
        {
            AStar.Pathfinding(start, goal, result);
        }
    }
}