using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class EnemySpawner : DungeonEntity
    {
        [System.Serializable]
        class Data
        {
            [SerializeField] Enemy _prefab;
            [SerializeField] int _weight;

            public Enemy Prefab => _prefab;
            public int Weight => _weight;
        }

        [SerializeField] Data[] _data;

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
                _spawned = Instantiate(Choice());
                _spawned.Initialize(Coords);

                yield return _waitDefeated ??= new WaitUntil(IsDefeated);
                
                // 敵が撃破されてから再度湧くまでの間隔。適当に時間を指定。
                yield return _waitInterval ??= new WaitForSeconds(10.0f);
            }
        }

        Enemy Choice()
        {
            int sum = _data.Select(d => d.Weight).Sum();
            int random = Random.Range(0, sum) + 1;
            foreach (Data data in _data)
            {
                int range = sum - data.Weight;
                if (range < random) return data.Prefab;

                sum = range;
            }

            // 確率の設定に異常がある場合は、一番出やすい敵を返す。
            return _data.OrderByDescending(d => d.Weight).FirstOrDefault().Prefab;
        }

        bool IsDefeated()
        {
            return _spawned == null;
        }
    }
}
