using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.EnemyComponent
{
    public class ActionSelector : MonoBehaviour
    {
        Enemy _enemy;

        void Awake()
        {
            _enemy = GetComponent<Enemy>();
        }

        public string Select()
        {
            // ���g�̏㉺���E�ɖ`���҂�����΍U������B
            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    if (i == 0 && k == 0) continue;
                    if (i != 0 && k != 0) continue;

                    Cell cell = DungeonManager.GetCell(_enemy.Coords + new Vector2Int(k, i));
                    foreach (Actor actor in cell.GetActors())
                    {
                        if (actor is Adventurer) return "Attack";
                    }
                }
            }

            List<Vector2Int> candidate = new List<Vector2Int>();

            // �ړ���̌��ƂȂ���W�B�������ꂽ���W�̎���8�̃Z���܂ł��ړ��͈́B
            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    int nx = _enemy.SpawnCoords.x + k;
                    int ny = _enemy.SpawnCoords.y + i;

                    if (Blueprint.Base[ny][nx] == '_')
                    {
                        candidate.Add(new Vector2Int(nx, ny));
                    }
                }
            }

            // ���ƂȂ���W�̂����A�ʍs�s�\�������͎΂ߕ����̍��W�������B
            for (int i = candidate.Count - 1; i >= 0; i--)
            {
                Cell cell = DungeonManager.GetCell(candidate[i]);
                if (cell.IsImpassable()) candidate.RemoveAt(i);
                else if (1.4f <= Vector2Int.Distance(_enemy.Coords, candidate[i])) candidate.RemoveAt(i);
            }

            // �c�������W�̒����烉���_���ɑI�сA���̍��W�ֈړ����������Ԃ��B
            Vector2Int choice = candidate[Random.Range(0, candidate.Count)];
            Vector2Int direction = choice - _enemy.Coords;          
            if (direction == Vector2Int.zero) return "Idle";
            if (direction == Vector2Int.up) return "MoveNorth";
            if (direction == Vector2Int.down) return "MoveSouth";
            if (direction == Vector2Int.right) return "MoveEast";
            if (direction == Vector2Int.left) return "MoveWest";

            Debug.LogError("�GAI���������s����I���o���Ă��Ȃ��B");

            return string.Empty;
        }
    }
}
