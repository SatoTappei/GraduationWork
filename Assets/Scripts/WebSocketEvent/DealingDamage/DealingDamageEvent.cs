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

            // まだダンジョン内にいる冒険者のみを狙う。
            Adventurer[] alive = _spawner.Spawned.Where(a => !a.IsCompleted).ToArray();
            if (alive.Length == 0) return;

            // ランダムな冒険者に対を選び、狙うことが出来るセルに配置する。
            int random = Random.Range(0, alive.Length);
            Adventurer target = alive[random];
            Vector2Int coords = GetOptimalCoords(target);
            Cell placeCell = DungeonManager.GetCell(coords);

            if (_effectpool.TryPop(out DealingDamageEffect effect))
            {
                // 壁の上に生成する。
                effect.Play(placeCell.Position + Vector3.up, target);
            }
        }

        // 目標とある程度離れた座標を返す。
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

        // 候補となる座標を描画する。
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