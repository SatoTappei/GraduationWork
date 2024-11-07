using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface ITalkable
    {
        public void Talk(BilingualString text, string source, Vector2Int coords);
    }
}
