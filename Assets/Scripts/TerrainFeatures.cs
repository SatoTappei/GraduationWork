using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public class TerrainFeatures : MonoBehaviour
    {
        [System.Serializable]
        class Data
        {
            [SerializeField] Vector2Int _coords;
            [SerializeField] BilingualString _text;

            public Vector2Int Coords => _coords;
            public BilingualString Text => _text;
        }

        [SerializeField] Data[] _data;

        public void Draw()
        {
            DungeonManager dungeonManager = DungeonManager.Find();

            if (!Application.isPlaying || dungeonManager == null || _data == null) return;

            for (int i = 0; i < _data.Length; i++)
            {
                Cell cell = dungeonManager.GetCell(_data[i].Coords);

                Gizmos.color = Color.red;
                Gizmos.DrawCube(cell.Position, Vector3.one);

                // ˆÊ’u‚Ìã‚É”Ô†‚ð•\Ž¦‚µ‚Ä‚¨‚­B
                GUIStyle labelStyle = new GUIStyle() { normal = { textColor = Color.white } };
                Handles.Label(cell.Position + Vector3.up, i.ToString(), labelStyle);
            }
        }
    }
}
