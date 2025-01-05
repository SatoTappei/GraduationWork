using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PathfindingDebugger : MonoBehaviour
    {
        [SerializeField] Vector2Int _start;
        [SerializeField] Vector2Int _goal;

        List<Cell> _path;

        void Awake()
        {
            _path = new List<Cell>();
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                PathfindingTest(_start, _goal);
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                StartCoroutine(ErrorCheckTest());
            }
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                ProcessingTimeTest();
            }
        }

        void OnDrawGizmosSelected()
        {
            DrawPath();
        }

        // 任意の座標間の経路をチェック。
        void PathfindingTest(Vector2Int a, Vector2Int b)
        {
            _path.Clear();

            Pathfinding pathfinding = Pathfinding.Find();
            pathfinding.CalculatePath(a, b, _path);
        }

        // 非同期で経路を全探索してエラーが出ないかチェック。
        IEnumerator ErrorCheckTest()
        {
            Pathfinding pathfinding = Pathfinding.Find();
            for (int i = 0; i < Blueprint.Height; i++)
            {
                for (int k = 0; k < Blueprint.Width; k++)
                {
                    for (int m = 0; m < Blueprint.Height; m++)
                    {
                        for (int n = 0; n < Blueprint.Width; n++)
                        {
                            _start = new Vector2Int(k, i);
                            _goal = new Vector2Int(n, m);
                            _path.Clear();

                            pathfinding.CalculatePath(_start, _goal, _path);
                            yield return null;
                            pathfinding.CalculatePath(_goal, _start, _path);
                            yield return null;
                        }
                    }
                }
            }
        }

        // 同期的に経路を全探索して処理時間を計測。
        void ProcessingTimeTest()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Pathfinding pathfinding = Pathfinding.Find();
            for (int i = 0; i < Blueprint.Height; i++)
            {
                for (int k = 0; k < Blueprint.Width; k++)
                {
                    for (int m = 0; m < Blueprint.Height; m++)
                    {
                        for (int n = 0; n < Blueprint.Width; n++)
                        {
                            _start = new Vector2Int(k, i);
                            _goal = new Vector2Int(n, m);
                            _path.Clear();

                            pathfinding.CalculatePath(_start, _goal, _path);
                        }
                    }
                }
            }

            sw.Stop();
            Debug.Log(sw.ElapsedMilliseconds);
        }

        void DrawPath()
        {
            if (_path == null) return;

            Vector3 cubeSize = new Vector3(0.5f, 33.0f, 0.5f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(DungeonManager.GetCell(_start).Position, cubeSize);
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(DungeonManager.GetCell(_goal).Position, cubeSize);

            Gizmos.color = Color.blue;
            foreach (Cell c in _path)
            {
                Gizmos.DrawSphere(c.Position, 0.5f);
            }
        }
    }
}
