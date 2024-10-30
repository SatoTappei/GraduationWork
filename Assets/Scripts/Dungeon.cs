using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Dungeon : MonoBehaviour
    {
        [System.Serializable]
        public class Prefabs
        {
            [SerializeField] DungeonEntity _entrance;
            [SerializeField] DungeonEntity _door;
            [SerializeField] DungeonEntity _treasure;
            [SerializeField] DungeonEntity _healingSpot;
            [SerializeField] DungeonEntity _enemySpawner;
            [SerializeField] DungeonEntity _barrel;
            [SerializeField] DungeonEntity _container;

            public DungeonEntity Entrance => _entrance;
            public DungeonEntity Door => _door;
            public DungeonEntity Treasure => _treasure;
            public DungeonEntity HealingSpot => _healingSpot;
            public DungeonEntity EnemySpawner => _enemySpawner;
            public DungeonEntity Barrel => _barrel;
            public DungeonEntity Container => _container;
        }

        [SerializeField] Prefabs _prefabs;

        Cell[,] _grid;

        public Cell[,] Grid
        {
            get
            {
                CreateIfNull();
                return _grid;
            }
        }

        void Start()
        {
            CreateIfNull();
        }

        void CreateIfNull()
        {
            if (_grid == null) Create();
        }

        void Create()
        {
            CreateGrid();
            BuildNonDirectionalEntity('<', _prefabs.Entrance);
            BuildDirectionalEntity(Blueprint.Doors, _prefabs.Door);
            BuildDirectionalEntity(Blueprint.Treasures, _prefabs.Treasure);
            BuildNonDirectionalEntity('h', _prefabs.HealingSpot);
            BuildNonDirectionalEntity('e', _prefabs.EnemySpawner);
            BuildNonDirectionalEntity('B', _prefabs.Barrel);
            BuildNonDirectionalEntity('C', _prefabs.Container);
        }

        void CreateGrid()
        {
            int h = Blueprint.Height;
            int w = Blueprint.Width;
            Vector3 basePosition = transform.position;

            _grid = new Cell[h, w];
            for (int i = 0; i < h; i++)
            {
                for (int k = 0; k < w; k++)
                {
                    _grid[i, k] = GetNewCell(basePosition, k, i);
                }
            }
        }

        static Cell GetNewCell(Vector3 basePosition, int x, int y)
        {
            Vector3 position = basePosition + new Vector3(x, 0, y);
            int cost = GetCellCost(x, y);
            Terrain terrain = GetCellTerrain(x, y);
            Location location = GetCellLocation(x, y);
            return new Cell(position, x, y, cost, terrain, location);
        }

        static int GetCellCost(int x, int y)
        {
            // 現状コストの概念が無い。
            return 1;
        }

        static Terrain GetCellTerrain(int x, int y)
        {
            char c = Blueprint.Base[y][x];
            if (c == '#') return Terrain.Wall;
            if (c == '_') return Terrain.Floor;

            Debug.LogWarning($"文字に対応するTerrainが存在しない。: {new Vector2Int(x, y)}{c}");
            return Terrain.None;
        }

        static Location GetCellLocation(int x, int y)
        {
            char c = Blueprint.Location[y][x];
            if (c == '0') return Location.Corridor;
            if (c == '1') return Location.Room;
            if (c == '2') return Location.EntranceHall;
            if (c == '3') return Location.TreasureVault;
            if (c == '4') return Location.Prison;
            if (c == '5') return Location.Arena;
            if (c == '#') return Location.Wall;

            Debug.LogWarning($"文字に対応するLocationが存在しない。: {new Vector2Int(x, y)}{c}");
            return Location.None;
        }

        static void BuildDirectionalEntity(string[] blueprint, DungeonEntity prefab)
        {
            Build(blueprint, '8', '2', '4', '6', prefab);
        }

        static void BuildNonDirectionalEntity(char letter, DungeonEntity prefab)
        {
            Build(Blueprint.Interaction, letter, letter, letter, letter, prefab);
        }

        static void Build(string[] blueprint, char up, char down, char left, char right, DungeonEntity prefab)
        {
            for (int i = 0; i < Blueprint.Height; i++)
            {
                for (int k = 0; k < Blueprint.Width; k++)
                {
                    char symbol = blueprint[i][k];

                    // 1つのブロックに上下左右の向きがあり、それぞれ対応する文字が違う。
                    if (!$"{up}{down}{left}{right}".Contains(symbol)) continue;

                    DungeonEntity entity = Instantiate(prefab);
                    entity.Place(k, i, GetDirection(symbol));
                }
            }

            Vector2Int GetDirection(char symbol)
            {
                if (symbol == up) return Vector2Int.up;
                if (symbol == down) return Vector2Int.down;
                if (symbol == left) return Vector2Int.left;
                if (symbol == right) return Vector2Int.right;
                
                return default;
            }
        }

        public void Draw()
        {
            if (_grid == null) return;
            foreach (Cell c in _grid) c.Draw();
        }
    }
}