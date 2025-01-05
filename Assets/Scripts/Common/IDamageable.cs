using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IDamageable
    {
        public string Damage(int value, Vector2Int coords, string effect = "");
    }
}
