using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public enum Terrain
    {
        Floor,
        Wall,
        // ‚±‚±‚É’Ç‰Á
    }

    public class Node
    {
        public Node(Vector3 position, int x, int y, int cost, Terrain terrain)
        {
            Position = position;
            X = x;
            Y = y;
            Cost = cost;
            Terrain = terrain;
        }

        public Vector3 Position { get; }
        public int X { get; }
        public int Y { get; }
        public int Cost { get; }
        public Terrain Terrain { get; }

        public Node Parent { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }

        public void Draw()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(Position, Vector3.one * 0.75f);
        }
    }
}
