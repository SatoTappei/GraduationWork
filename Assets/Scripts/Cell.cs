using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum Terrain
    {
        Floor,
        Wall,
        Passable,
        Impassable,
        // Ç±Ç±Ç…í«â¡
    }

    public class Cell
    {
        List<Actor> _actors;

        public Cell(Vector3 position, int x, int y, int cost, Terrain terrain)
        {
            Position = position;
            X = x;
            Y = y;
            Cost = cost;
            Terrain = terrain;
            _actors = new List<Actor>();
        }

        public Vector3 Position { get; }
        public int X { get; }
        public int Y { get; }
        public int Cost { get; }
        public Terrain Terrain { get; }
        public Vector2Int Coords => new Vector2Int(X, Y);

        public Cell Parent { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost;

        public bool IsPassable()
        {
            return Terrain == Terrain.Floor || Terrain == Terrain.Passable;
        }

        public bool IsImpassable()
        {
            return !IsPassable();
        }

        public void AddActor(Actor actor)
        {
            if (_actors.Contains(actor))
            {
                Debug.LogWarning($"{Coords}: {actor.ID}ÇÕä˘Ç…í«â¡çœÇ›ÅB");
            }
            else _actors.Add(actor);
        }

        public void RemoveActor(Actor actor)
        {
            _actors.Remove(actor);
        }

        public IReadOnlyList<Actor> GetActors()
        {
            return _actors;
        }

        public void Draw()
        {
            Gizmos.color = IsPassable() ? Color.green : Color.red;
            Gizmos.DrawCube(Position, Vector3.one * 0.75f);
        }
    }
}