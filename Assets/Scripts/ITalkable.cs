using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface ITalkable
    {
        public void Talk(string id, string topic, Vector2Int coords);
    }
}
