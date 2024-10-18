using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Enemy : Character
    {
        public abstract void Place(Vector2Int coords);
    }
}
