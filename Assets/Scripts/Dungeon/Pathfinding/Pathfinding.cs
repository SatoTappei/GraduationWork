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

        public static Pathfinding Find()
        {
            return GameObject.FindGameObjectWithTag("DungeonManager").GetComponent<Pathfinding>();
        }

        public bool CalculatePath(Vector2Int start, Vector2Int goal, List<Cell> result)
        {
            if (_aStar == null) Initialize();

            return _aStar.Pathfinding(start, goal, result);
        }

        void Initialize()
        {
            Dungeon dungeon = GetComponent<Dungeon>();
            if (dungeon.Grid == null)
            {
                Debug.LogWarning("�o�H�T���@�\�̏������ɕK�v�ȁA�_���W�����̃O���b�h�������ł��Ă��Ȃ��B");
            }
            else
            {
                _aStar = new AStar(dungeon.Grid);
            }
        }
    }
}