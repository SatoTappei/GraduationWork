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

            // �擪�̍��W���珇�ɔz�u���Ă������_���ɔz�u�����悤�A���ёւ���B
            _candidateCoords = _candidateCoords.OrderBy(_ => Random.Range(0, int.MaxValue)).ToArray();
        }

        void OnDrawGizmosSelected()
        {
            if (_isDraw) Draw();
        }

        public void Execute()
        {
            if (!_trapPool.TryPop(out Trap trap)) return;

            // ���W�����Ȃ��悤�ɔz�u����B
            for (int i = 0; i < _candidateCoords.Length; i++)
            {
                if (_placedCoords.Contains(_candidateCoords[i])) continue;
                else _placedCoords.Add(_candidateCoords[i]);

                trap.Place(_candidateCoords[i], Vector2Int.up);

                break;
            }
        }

        // �C�x���g�̌��ƂȂ���W��`�悷��B
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
