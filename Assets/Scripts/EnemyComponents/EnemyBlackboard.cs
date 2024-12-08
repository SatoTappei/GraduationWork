using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EnemyBlackboard : MonoBehaviour
    {
        [SerializeField] int _maxHp = 100;

        public Vector2Int SpawnCoords { get; set; }
        public Vector2Int Coords { get; set; }
        public Vector2Int Direction { get; set; }
        public int CurrentHp { get; set; }

        public int MaxHp => _maxHp;
        public bool IsDefeated => CurrentHp <= 0;
        public bool IsAlive => !IsDefeated;
    }
}
