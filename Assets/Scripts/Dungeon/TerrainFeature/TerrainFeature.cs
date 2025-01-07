using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public class TerrainFeature : MonoBehaviour
    {
        public static TerrainFeature Find()
        {
            return GameObject.FindGameObjectWithTag("DungeonManager").GetComponent<TerrainFeature>();
        }

        public bool TryGet(Vector2Int coords, out IReadOnlyList<Information> result)
        {
            TerrainNavigator.TryGet(coords, out result);
            string[] s = result.Select(r => r.Text.Japanese).ToArray();
            Debug.Log(string.Join("\n", s));

            return false;
        }
    }
}
