using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class LeverBind
    {
        static Dictionary<Vector2Int, Vector2Int[]> Bind = new Dictionary<Vector2Int, Vector2Int[]>()
        {
            { 
                new Vector2Int(16, 8), // レバーの座標 
                new Vector2Int[4]      // レバーで起動するオブジェクトの座標。
                { 
                    new Vector2Int(11, 1), 
                    new Vector2Int(22, 1), 
                    new Vector2Int(1, 8), 
                    new Vector2Int(8, 10) 
                }
            },
            {
                new Vector2Int(10, 12),
                new Vector2Int[1]
                { 
                    new Vector2Int(12, 15) 
                }
            },
            {
                new Vector2Int(8, 1),
                new Vector2Int[3]
                {
                    new Vector2Int(22, 11),
                    new Vector2Int(22, 13),
                    new Vector2Int(1, 15)
                }
            }
        };

        public static IReadOnlyList<Vector2Int> GetTargetCoords(Vector2Int leverCoords)
        {
            if (Bind.TryGetValue(leverCoords, out Vector2Int[] targetCoords))
            {
                return targetCoords;
            }
            else
            {
                return new List<Vector2Int>();
            }
        }
    }
}