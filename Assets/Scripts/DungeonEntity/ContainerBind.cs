using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class ContainerBind
    {
        class Data
        {
            public float Interval;
            public string[] Items;
        }

        static Dictionary<Vector2Int, Data> Bind = new Dictionary<Vector2Int, Data>()
        {
            {
                new Vector2Int(3, 1),
                new Data()
                {
                    Interval = 30, // 単位は秒。 
                    Items = new string[] { "クラッカー", "手榴弾", "荷物", "ガラクタ" }
                }
            },
            {
                new Vector2Int(7, 5),
                new Data()
                {
                    Interval = 120,
                    Items = new string[] { "錆びた剣", "ヘルメット" }
                }
            },
            {
                new Vector2Int(8, 5),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "手榴弾", "荷物", "ガラクタ" }
                }
            },
            {
                new Vector2Int(18, 5),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "壊れた罠", "荷物" }
                }
            },
            {
                new Vector2Int(14, 11),
                new Data()
                {
                    Interval = 10,
                    Items = new string[] { "手榴弾", "ガラクタ" }
                }
            },
            {
                new Vector2Int(15, 3),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "手榴弾", "荷物", "ガラクタ" }
                }
            },
            {
                new Vector2Int(20, 3),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "ガラクタ" }
                }
            },
            {
                new Vector2Int(17, 5),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "切れた電球", "荷物" }
                }
            },
            {
                new Vector2Int(16, 13),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "荷物", "手榴弾" }
                }
            },
            {
                new Vector2Int(16, 14),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "クラッカー", "ガラクタ" }
                }
            },
            {
                new Vector2Int(10, 17),
                new Data()
                {
                    Interval = 10,
                    Items = new string[] { "クラッカー", "錆びた剣", "切れた電球", "ヘルメット" }
                }
            },
        };

        public static float GetInterval(Vector2Int coords)
        {
            if (Bind.TryGetValue(coords, out Data data))
            {
                return data.Interval;
            }
            else
            {
                return 0;
            }
        }

        public static IReadOnlyList<string> GetItems(Vector2Int coords)
        {
            if (Bind.TryGetValue(coords, out Data data))
            {
                return data.Items;
            }
            else
            {
                return new List<string>();
            }
        }
    }
}