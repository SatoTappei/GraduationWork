using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class DealingDamageEvent : MonoBehaviour
    {
        [SerializeField] Vector2Int[] _candidateCoords;
        [SerializeField] bool _isDraw;

        DealingDamageEffectPool _effectpool;
        AdventurerSpawner _spawner;

        void Awake()
        {
            _effectpool = GetComponent<DealingDamageEffectPool>();
            _spawner = AdventurerSpawner.Find();
        }

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying && _isDraw) Draw();
        }

        public void Execute()
        {
            if (_spawner.Spawned.Count == 0) return;

            // �܂��_���W�������ɂ���`���҂݂̂�_���B
            Adventurer[] alive = _spawner.Spawned.Where(a => !a.IsCompleted).ToArray();
            if (alive.Length == 0) return;

            // �����_���Ȗ`���҂ɑ΂�I�сA�_�����Ƃ��o����Z���ɔz�u����B
            int random = Random.Range(0, alive.Length);
            Adventurer target = alive[random];
            Vector2Int coords = GetOptimalCoords(target);
            Cell placeCell = DungeonManager.GetCell(coords);

            if (_effectpool.TryPop(out DealingDamageEffect effect))
            {
                // �ǂ̏�ɐ�������B
                effect.Play(placeCell.Position + Vector3.up, target);
            }
        }

        // �ڕW�Ƃ�����x���ꂽ���W��Ԃ��B
        Vector2Int GetOptimalCoords(Adventurer target)
        {
            const float Threshold = 5.0f;

            float minDistance = float.MaxValue;
            Vector2Int minCoords = default;
            for (int i = 0; i < _candidateCoords.Length; i++)
            {
                float distance = Vector2.Distance(target.Coords, _candidateCoords[i]);
                if (distance > Threshold && distance < minDistance)
                {
                    minDistance = distance;
                    minCoords = _candidateCoords[i];
                }
            }

            return minCoords;
        }

        // ���ƂȂ���W��`�悷��B
        void Draw()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            for (int i = 0; i < _candidateCoords.Length; i++)
            {
                Cell cell = DungeonManager.GetCell(_candidateCoords[i]);
                Gizmos.DrawCube(cell.Position + Vector3.up, Vector3.one);
            }
        }
    }
}