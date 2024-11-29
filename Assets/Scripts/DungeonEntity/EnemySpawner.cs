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
                
                // �G�����j����Ă���ēx�N���܂ł̊Ԋu�B�K���Ɏ��Ԃ��w��B
                yield return _waitInterval ??= new WaitForSeconds(10.0f);
            }
        }

        bool IsDefeated()
        {
            return _spawned == null;
        }
    }
}
