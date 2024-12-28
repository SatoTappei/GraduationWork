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
        DungeonManager _dungeonManager;
        GameLog _gameLog;
        HashSet<Vector2Int> _placedCoords;

        void Awake()
        {
            _trapPool = GetComponent<TrapPool>();
            DungeonManager.TryFind(out _dungeonManager);
            GameLog.TryFind(out _gameLog);
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

            // イベント実行をログに表示。
            _gameLog.Add("システム", "何者かが罠を設置した。", GameLogColor.Green);
        }

        // イベントの候補となる座標を描画する。
        void Draw()
        {
            if (_dungeonManager == null) return;

            Gizmos.color = new Color(1, 0, 0, 0.5f);
            for (int i = 0; i < _candidateCoords.Length; i++)
            {
                Cell cell = _dungeonManager.GetCell(_candidateCoords[i]);
                Gizmos.DrawCube(cell.Position, Vector3.one);
            }
        }
    }
}
