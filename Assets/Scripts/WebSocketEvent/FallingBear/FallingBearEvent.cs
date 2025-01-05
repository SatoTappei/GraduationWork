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

            // ランダムな冒険者に対し、狙うことが出来るセル。
            int random = Random.Range(0, _spawner.Spawned.Count);
            Adventurer target = _spawner.Spawned[random];
            Cell placeCell = DungeonManager.GetCell(target.Coords);

            if (_effectPool.TryPop(out FallingBearEffect effect))
            {
                // 目標の頭上に生成する。高さは適当。
                effect.Play(placeCell.Position + Vector3.up * 3.0f, target);
            }

            // イベント実行をログに表示。
            GameLog.Add("システム", "何者かが冒険者にいたずらした。", GameLogColor.Green);
        }
    }
}
