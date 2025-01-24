using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class HelicopterCrashEvent : MonoBehaviour
    {
        HelicopterCrashEffectPool _effectPool;
        AdventurerSpawner _spawner;

        void Awake()
        {
            _effectPool = GetComponent<HelicopterCrashEffectPool>();
            _spawner = AdventurerSpawner.Find();
        }

        public void Execute()
        {
            if (_spawner.Spawned.Count == 0) return;

            // ランダムな冒険者に対し、狙うことが出来る座標のセル。
            int random = Random.Range(0, _spawner.Spawned.Count);
            Adventurer target = _spawner.Spawned[random];
            Cell placeCell = DungeonManager.GetCell(target.Coords);

            if (_effectPool.TryPop(out HelicopterCrashEffect effect))
            {
                // 目標の頭上に生成する。高さは適当。
                effect.Play(placeCell.Position + Vector3.up * 3.0f, target);
            }
        }
    }
}
