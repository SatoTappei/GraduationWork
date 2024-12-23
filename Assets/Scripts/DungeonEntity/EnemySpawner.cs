using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EnemySpawner : DungeonEntity
    {
        [SerializeField] Enemy _prefab;

        Enemy _spawned;
        WaitUntil _waitDefeated;
        WaitForSeconds _waitInterval;

        void Start()
        {
            StartCoroutine(UpdateAsync());
        }

        IEnumerator UpdateAsync()
        {
            while (true)
            {
                _spawned = Instantiate(_prefab);
                _spawned.Initialize(Coords);

                yield return _waitDefeated ??= new WaitUntil(IsDefeated);
                
                // 敵が撃破されてから再度湧くまでの間隔。適当に時間を指定。
                yield return _waitInterval ??= new WaitForSeconds(10.0f);
            }
        }

        bool IsDefeated()
        {
            return _spawned == null;
        }
    }
}
