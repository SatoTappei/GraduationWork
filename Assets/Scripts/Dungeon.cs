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
            [SerializeField] DungeonEntity _enemyKadukiSpawner;
            [SerializeField] DungeonEntity _barrel;
            [SerializeField] DungeonEntity _container;

            public DungeonEntity Entrance => _entrance;
            public DungeonEntity Door => _door;
            public DungeonEntity Treasure => _treasure;
            public DungeonEntity HealingSpot => _healingSpot;
            public DungeonEntity EnemyKadukiSpawner => _enemyKadukiSpawner;
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
            BuildNonDirectionalEntity('k', _prefabs.EnemyKadukiSpawner);
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
            return new Cell(position, x, y, cost, terrain);
        }

        static int GetCellCost(int x, int y)
        {
            // ����R�X�g�̊T�O�������B
            return 1;
        }

        static Terrain GetCellTerrain(int x, int y)
        {
            if (Blueprint.Base[y][x] == '#') return Terrain.Wall;

            // �󔠂͏㉺���E�̌���������̂�4��ނ̕����Ŕ���B
            bool isTreasure = "2468".Contains(Blueprint.Treasures[y][x]);
            if (isTreasure) return Terrain.Impassable;

            // �ꉞ"�ʍs�\"�̒n�`����������A����ȃ��[�����K�v�ɂȂ�܂ł͔���G�̗N���n�_���������B
            // ���W�����ꂼ���Blueprint�Ŕ��肷��K�v������̂Ŗʓ|�������B
            return Terrain.Floor;
        }

        void BuildDirectionalEntity(string[] blueprint, DungeonEntity prefab)
        {
            Build(blueprint, '8', '2', '4', '6', prefab);
        }

        void BuildNonDirectionalEntity(char letter, DungeonEntity prefab)
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

                    // 1�̃u���b�N�ɏ㉺���E�̌���������A���ꂼ��Ή����镶�����Ⴄ�B
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