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
        [SerializeField] TerrainFeaturesData _data;
        [SerializeField] bool _isDrawGizmos = true;

        Dictionary<Vector2Int, List<SharedInformation>> _table;

        void Awake()
        {
            ConvertDataToTable();
        }

        void OnDrawGizmosSelected()
        {
            if (_isDrawGizmos) Draw();
        }

        public bool TryGetInformation(Vector2Int coords, out IReadOnlyList<SharedInformation> result)
        {
            if (_table.TryGetValue(coords, out List<SharedInformation> list))
            {
                // 呼び出し元で中身の値を弄っても影響が出ないよう、コピーして渡す。
                List<SharedInformation> temp = new List<SharedInformation>();
                foreach (SharedInformation e in list)
                {
                    SharedInformation copy = new SharedInformation(e.Text, e.Source, e.RemainingTurn);
                    temp.Add(copy);
                }

                result = temp;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        void ConvertDataToTable()
        {
            _table = new Dictionary<Vector2Int, List<SharedInformation>>();

            foreach (TerrainFeaturesData.Data data in _data.AllData)
            {
                _table.TryAdd(data.Coords, new List<SharedInformation>());
                _table[data.Coords].Add(data.ToSharedInformation());
            }
        }

        void Draw()
        {
            if (_table == null) return;
            if (!DungeonManager.TryFind(out DungeonManager dungeonManager)) return;

            foreach (var pair in _table)
            {
                Cell cell = dungeonManager.GetCell(pair.Key);
                Gizmos.color = Color.red;
                Gizmos.DrawCube(cell.Position, Vector3.one);

                // セル上に設定した文字列を表示。
                GUIStyle labelStyle = new GUIStyle() { normal = { textColor = Color.white } };
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    string text = pair.Value[i].Text.Japanese;
                    Vector3 offset = Vector3.up * (i + 1);
                    Handles.Label(cell.Position + offset, text, labelStyle);
                }
            }
        }
    }
}
