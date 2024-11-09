using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        [SerializeField] bool _isDrawGizmos = true;

        Dictionary<Vector2Int, BilingualString> _table;

        void Awake()
        {
            _table = _data.ToDictionary(k => k.Coords, v => v.Text);
        }

        void OnDrawGizmosSelected()
        {
            if (_isDrawGizmos) Draw();
        }

        public bool TryGetInformation(Vector2Int coords, out SharedInformation info)
        {
            if (_table.TryGetValue(coords, out BilingualString text))
            {
                // 情報源はキャラクター自身という事にしておく。
                info = new SharedInformation(text, "Myself");
                return true;
            }
            else
            {
                info = null;
                return false;
            }
        }

        void Draw()
        {
            DungeonManager dungeonManager = DungeonManager.Find();

            if (!Application.isPlaying || dungeonManager == null || _data == null) return;

            for (int i = 0; i < _data.Length; i++)
            {
                Cell cell = dungeonManager.GetCell(_data[i].Coords);

                Gizmos.color = Color.red;
                Gizmos.DrawCube(cell.Position, Vector3.one);

                // 位置の上に番号を表示しておく。
                GUIStyle labelStyle = new GUIStyle() { normal = { textColor = Color.white } };
                Handles.Label(cell.Position + Vector3.up, i.ToString(), labelStyle);
            }
        }
    }
}
