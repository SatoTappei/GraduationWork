using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ActionResult
    {
        public ActionResult(string log, Vector2Int coords, Vector2Int direction)
        {
            Log = log;
            Coords = coords;
            Direction = direction;
        }

        // �s�����O�ɒǉ����镶�́B
        public string Log { get; }
        
        // �s����̍��W�ƌ����B
        public Vector2Int Coords { get; }
        public Vector2Int Direction { get; }
    }
}
