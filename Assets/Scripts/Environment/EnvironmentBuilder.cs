using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // プレハブ化してメッシュを結合するので、マップの更新時以外使わない。
    public class EnvironmentBuilder : MonoBehaviour
    {
        [SerializeField] GameObject _wallPrefab;
        [SerializeField] GameObject _floorPrefab;
        [SerializeField] GameObject _pillarPrefab;
        [SerializeField] GameObject _lampPrefab;

        void Start()
        {
            BuildWall(Blueprint.Base, _wallPrefab);
            BuildFloor(Blueprint.Base, _floorPrefab);
            BuildPillars(Blueprint.Pillars, _pillarPrefab);
            BuildLamp(Blueprint.Lamp, _lampPrefab);
        }

        static void BuildWall(string[] blueprint, GameObject prefab)
        {
            Build(blueprint, '#', '#', '#', '#', prefab);
        }

        static void BuildFloor(string[] blueprint, GameObject prefab)
        {
            Build(blueprint, '_', '_', '_', '_', prefab);
        }

        static void BuildPillars(string[] blueprint, GameObject prefab)
        {
            Build(blueprint, '7', '3', '9', '1', prefab);
        }

        static void BuildLamp(string[] blueprint, GameObject prefab)
        {
            Build(blueprint, '8', '2', '4', '6', prefab);
        }

        static void Build(string[] blueprint, char up, char down, char left, char right, GameObject prefab)
        {
            for (int i = 0; i < Blueprint.Height; i++)
            {
                for (int k = 0; k < Blueprint.Width; k++)
                {
                    char symbol = blueprint[i][k];

                    // 1つのブロックに上下左右の向きがあり、それぞれ対応する文字が違う。
                    if (!$"{up}{down}{left}{right}".Contains(symbol)) continue;

                    GameObject tile = Instantiate(prefab);
                    tile.transform.position = new Vector3(k, 0, i);

                    if (tile.TryGetComponent(out Environment e))
                    {
                        e.Initialize(blueprint, k, i);
                    }

                    float angle = 0;
                    if (symbol == up) angle = 0;
                    else if (symbol == down) angle = 180;
                    else if (symbol == left) angle = 90;
                    else if (symbol == right) angle = -90;

                    tile.transform.Rotate(Vector3.up * angle);
                }
            }
        }
    }
}
