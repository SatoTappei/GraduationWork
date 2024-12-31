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
        AdventurerSpawner _adventurerSpawner;

        void Awake()
        {
            _effectpool = GetComponent<DealingDamageEffectPool>();
            AdventurerSpawner.TryFind(out _adventurerSpawner);
        }

        void OnDrawGizmosSelected()
        {
            if (_isDraw) Draw();
        }

        public void Execute()
        {
            Adventurer[] adventurers = _adventurerSpawner.Spawned.Where(a => a != null).ToArray();
            
            if (adventurers.Length == 0) return;

            // �����_���Ȗ`���҂ɑ΂��A�_�����Ƃ��o������W�̃Z���B
            Adventurer target = adventurers[Random.Range(0, adventurers.Length)];
            Vector2Int coords = GetOptimalCoords(target);
            Cell placeCell = DungeonManager.GetCell(coords);

            if (_effectpool.TryPop(out DealingDamageEffect effect))
            {
                // �ǂ̏�ɐ�������B
                effect.Play(placeCell.Position + Vector3.up, target);
            }

            // �C�x���g���s�����O�ɕ\���B
            GameLog.Add("�V�X�e��", "���҂����`���҂�C�������B", GameLogColor.Green);
        }

        // �ڕW�Ƃ�����x���ꂽ���W��Ԃ��B
        Vector2Int GetOptimalCoords(Adventurer target)
        {
            const float DistanceThreshold = 5.0f;

            float minDistance = float.MaxValue;
            Vector2Int minCoords = default;
            for (int i = 0; i < _candidateCoords.Length; i++)
            {
                float distance = Vector2.Distance(target.Coords, _candidateCoords[i]);
                if (distance > DistanceThreshold && distance < minDistance)
                {
                    minDistance = distance;
                    minCoords = _candidateCoords[i];
                }
            }

            return minCoords;
        }

        // �C�x���g�̌��ƂȂ���W��`�悷��B
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