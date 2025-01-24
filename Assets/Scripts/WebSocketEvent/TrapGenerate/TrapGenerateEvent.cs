using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class TrapGenerateEvent : MonoBehaviour
    {
        [SerializeField] Vector2Int[] _candidateCoords;
        [SerializeField] bool _isDraw;

        TrapPool _trapPool;
        HashSet<Vector2Int> _placedCoords;

        void Awake()
        {
            _trapPool = GetComponent<TrapPool>();
            _placedCoords = new HashSet<Vector2Int>();

            // 先頭の座標から順に配置してもランダムに配置されるよう、並び替える。
            _candidateCoords = _candidateCoords.OrderBy(_ => Random.Range(0, int.MaxValue)).ToArray();
        }

        void OnDrawGizmosSelected()
        {
            if (_isDraw) Draw();
        }

        public void Execute()
        {
            if (!_trapPool.TryPop(out Trap trap)) return;

            // 座標が被らないように配置する。
            for (int i = 0; i < _candidateCoords.Length; i++)
            {
                if (_placedCoords.Contains(_candidateCoords[i])) continue;
                else _placedCoords.Add(_candidateCoords[i]);

                trap.Place(_candidateCoords[i], Vector2Int.up);

                break;
            }
        }

        // イベントの候補となる座標を描画する。
        void Draw()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            for (int i = 0; i < _candidateCoords.Length; i++)
            {
                Cell cell = DungeonManager.GetCell(_candidateCoords[i]);
                Gizmos.DrawCube(cell.Position, Vector3.one);
            }
        }
    }
}
