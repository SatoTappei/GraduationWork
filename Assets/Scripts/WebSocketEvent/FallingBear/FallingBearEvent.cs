using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class FallingBearEvent : MonoBehaviour
    {
        FallingBearEffectPool _effectPool;
        AdventurerSpawner _spawner;

        void Awake()
        {
            _effectPool = GetComponent<FallingBearEffectPool>();
            _spawner = AdventurerSpawner.Find();
        }

        public void Execute()
        {
            if (_spawner.Spawned.Count == 0) return;

            // まだダンジョン内にいる冒険者のみを狙う。
            Adventurer[] alive = _spawner.Spawned.Where(a => !a.IsCompleted).ToArray();
            if (alive.Length == 0) return;

            // ランダムな冒険者に対し、狙うことが出来るセル。
            int random = Random.Range(0, alive.Length);
            Adventurer target = alive[random];
            Cell placeCell = DungeonManager.GetCell(target.Coords);

            if (_effectPool.TryPop(out FallingBearEffect effect))
            {
                // 目標の頭上に生成する。高さは適当。
                effect.Play(placeCell.Position + Vector3.up * 3.0f, target);
            }
        }
    }
}
