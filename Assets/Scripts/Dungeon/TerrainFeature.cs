using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public class TerrainFeature : MonoBehaviour
    {
        // BilingualString�^�ł͂Ȃ��Astring�^��2�Ȃ̂ŁA�G�f�B�^�[����l��ݒ肷��ۂɃg�O�����X�g�ɂȂ炸�A���₷���B
        [System.Serializable]
        public class Data
        {
            [SerializeField] Vector2Int _coords;
            [SerializeField] string _japanese;
            [SerializeField] string _english;
            [SerializeField] int _validTurn;

            public Vector2Int Coords => _coords;
            public string Japanese => _japanese;
            public string English => _english;
            public int ValidTurn => _validTurn;

            public Information ToInformation()
            {
                // ���̏�񂪐M���ł��邩�̔��f��AI�ɔC���邽�߁AScore�̒l�͐ݒ肵�Ȃ��B
                return new Information(Japanese, English, "System", ValidTurn, true);
            }
        }

        [SerializeField] Data[] _data;
        [SerializeField] bool _isDrawGizmos = true;

        Dictionary<Vector2Int, List<Information>> _table;

        void Awake()
        {
            ConvertDataToTable();
        }

        void OnDrawGizmosSelected()
        {
            if (_isDrawGizmos) Draw();
        }

        public bool TryGetInformation(Vector2Int coords, out IReadOnlyList<Information> result)
        {
            if (_table.TryGetValue(coords, out List<Information> list))
            {
                // �Ăяo�����Œ��g�̒l��M���Ă��e�����o�Ȃ��悤�A�R�s�[���ēn���B
                List<Information> temp = new List<Information>();
                foreach (Information e in list)
                {
                    Information copy = new Information(e.Text, e.Source, e.RemainingTurn, true);
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
            _table = new Dictionary<Vector2Int, List<Information>>();

            foreach (Data data in _data)
            {
                _table.TryAdd(data.Coords, new List<Information>());
                _table[data.Coords].Add(data.ToInformation());
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

#if UNITY_EDITOR
                // �Z����ɐݒ肵���������\���B
                GUIStyle labelStyle = new GUIStyle() { normal = { textColor = Color.white } };
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    string text = pair.Value[i].Text.Japanese;
                    Vector3 offset = Vector3.up * (i + 1);
                    Handles.Label(cell.Position + offset, text, labelStyle);
                }
#endif
            }
        }
    }
}
