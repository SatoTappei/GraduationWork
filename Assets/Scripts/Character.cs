using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Character : Actor, IDamageable
    {
        public virtual string Damage(string id, string weapon, int value, Vector2Int coords)
        {
            return string.Empty;
        }
    }
}
