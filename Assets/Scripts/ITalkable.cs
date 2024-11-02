using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface ITalkable
    {
        public void Talk(string id, BilingualString text, Vector2Int coords);
    }
}
