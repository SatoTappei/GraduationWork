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
        DungeonManager _dungeonManager;
        UiManager _uiManager;

        void Awake()
        {
            _effectpool = GetComponent<DealingDamageEffectPool>();
            AdventurerSpawner.TryFind(out _adventurerSpawner);
            DungeonManager.TryFind(out _dungeonManager);
            UiManager.TryFind(out _uiManager);
        }

        void OnDrawGizmosSelected()
        {
            if (_isDraw) Draw();
        }

        public void Execute()
        {
            Adventurer[] adventurers = _adventurerSpawner.Spawned.Where(a => a != null).ToArray();
            
            if (adventurers.Length == 0) return;

            // ランダムな冒険者に対し、狙うことが出来る座標のセル。
            Adventurer target = adventurers[Random.Range(0, adventurers.Length)];
            Vector2Int coords = GetOptimalCoords(target);
            Cell placeCell = _dungeonManager.GetCell(coords);

            if (_effectpool.TryPop(out DealingDamageEffect effect))
            {
                // 壁の上に生成する。
                effect.Play(placeCell.Position + Vector3.up, target);
            }

            // イベント実行をログに表示。
            _uiManager.AddLog("システム", "何者かが冒険者を砲撃した。", GameLogColor.Green);
        }

        // 目標とある程度離れた座標を返す。
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

        // イベントの候補となる座標を描画する。
        void Draw()
        {
            if (_dungeonManager == null) return;

            Gizmos.color = new Color(1, 0, 0, 0.5f);
            for (int i = 0; i < _candidateCoords.Length; i++)
            {
                Cell cell = _dungeonManager.GetCell(_candidateCoords[i]);
                Gizmos.DrawCube(cell.Position + Vector3.up, Vector3.one);
            }
        }
    }
}