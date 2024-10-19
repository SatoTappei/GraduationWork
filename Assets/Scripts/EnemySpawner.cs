using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EnemySpawner : DungeonEntity
    {
        [SerializeField] Enemy _prefab;

        void Start()
        {
            Enemy enemy = Instantiate(_prefab);
            enemy.Place(Coords);
        }
    }
}
