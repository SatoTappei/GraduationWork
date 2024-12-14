using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum Terrain
    {
        None,
        Floor,
        Wall,
        // ここに追加
    }

    public enum TerrainEffect
    {
        None,
        Flaming,
    }

    public enum Location
    {
        None,
        Corridor,
        Room,
        EntranceHall,
        TreasureVault,
        Prison,
        Arena,
        Wall,
        // ここに追加
    }

    public class Cell
    {
        List<Actor> _actors;

        public Cell(Vector3 position, int x, int y, int cost, Terrain terrain, Location location)
        {
            _actors = new List<Actor>();
            Position = position;
            X = x;
            Y = y;
            Cost = cost;
            Terrain = terrain;
            Location = location;
            IsAvoid = false;
        }

        public Vector3 Position { get; }
        public int X { get; }
        public int Y { get; }
        public int Cost { get; }
        public Terrain Terrain { get; }
        public Location Location { get; }
        public Vector2Int Coords => new Vector2Int(X, Y);

        // 罠など自身の配置されているセル以外にも効果を及ぼすもの。
        public TerrainEffect TerrainEffect { get; set; }
        // 宝箱や戦闘中のキャラクターがいるマスなど、動的に状態が変化する場合に使用するフラグ。
        public bool IsAvoid { get; set; }

        public Cell Parent { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost;

        public bool IsPassable()
        {
            return Terrain == Terrain.Floor && !IsAvoid;
        }

        public bool IsImpassable()
        {
            return !IsPassable();
        }

        public void AddActor(Actor actor)
        {
            if (_actors.Contains(actor))
            {
                Debug.LogWarning($"{Coords}: {actor.ID}は既に追加済み。");
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