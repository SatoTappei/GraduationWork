using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class HelicopterCrashEvent : MonoBehaviour
    {
        HelicopterCrashEffectPool _effectPool;
        AdventurerSpawner _adventurerSpawner;

        void Awake()
        {
            _effectPool = GetComponent<HelicopterCrashEffectPool>();
            AdventurerSpawner.TryFind(out _adventurerSpawner);
        }

        public void Execute()
        {
            Adventurer[] adventurers = _adventurerSpawner.Spawned.Where(a => a != null).ToArray();

            if (adventurers.Length == 0) return;

            // ランダムな冒険者に対し、狙うことが出来る座標のセル。
            Adventurer target = adventurers[Random.Range(0, adventurers.Length)];
            Cell placeCell = DungeonManager.GetCell(target.Coords);

            if (_effectPool.TryPop(out HelicopterCrashEffect effect))
            {
                // 目標の頭上に生成する。高さは適当。
                effect.Play(placeCell.Position + Vector3.up * 3.0f, target);
            }

            // イベント実行をログに表示。
            GameLog.Add("システム", "何者かがヘリコプターを要請した。", GameLogColor.Green);
        }
    }
}
