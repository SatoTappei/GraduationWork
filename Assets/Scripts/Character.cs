using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Character : Actor, IDamageable
    {
        public virtual string Damage(int value, Vector2Int coords, string effect = "")
        {
            return string.Empty;
        }
    }
}
