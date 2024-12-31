using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DungeonManager : MonoBehaviour
    {
        static DungeonManager _instance;

        PlacedActors _placedActors;
        Dungeon _dungeon;

        void Awake()
        {
            if (_instance == null) _instance = this;
            else Destroy(this);

            _placedActors = new PlacedActors();
            _dungeon = GetComponent<Dungeon>();
        }

        void OnDestroy()
        {
            if (_instance = this) _instance = null;
        }

        public static void AddActorOnCell(Vector2Int coords, Actor actor)
        {
            GetCell(coords).AddActor(actor);
            _instance._placedActors.Add(actor);
        }

        public static void RemoveActorOnCell(Vector2Int coords, Actor actor)
        {
            GetCell(coords).RemoveActor(actor);
            _instance._placedActors.Remove(actor);
        }

        public static void SetCellTerrainEffect(Vector2Int coords, TerrainEffect effect)
        {
            GetCell(coords).TerrainEffect = effect;
        }

        public static void DeleteCellTerrainEffect(Vector2Int coords)
        {
            GetCell(coords).TerrainEffect = TerrainEffect.None;
        }

        public static void AddAvoidCell(Vector2Int coords)
        {
            GetCell(coords).IsAvoid = true;
        }

        public static void RemoveAvoidCell(Vector2Int coords)
        {
            GetCell(coords).IsAvoid = false;
        }

        public static IReadOnlyList<Actor> GetActorsOnCell(Vector2Int coords)
        {
            return GetCell(coords).GetActors();
        }

        public static IEnumerable<Cell> GetCells(string id)
        {
            foreach (Actor a in _instance._placedActors[id])
            {
                yield return GetCell(a.Coords);
            }
        }

        public static Cell GetCell(int x, int y)
        {
            Vector2Int coords = new Vector2Int(x, y);
            return GetCell(coords);
        }

        public static Cell GetCell(Vector2Int coords)
        {
            return _instance._dungeon.Grid[coords.y, coords.x];
        }
    }
}