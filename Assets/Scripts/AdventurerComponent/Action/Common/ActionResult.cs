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

        // 行動ログに追加する文章。
        public string Log { get; }
        
        // 行動後の座標と向き。
        public Vector2Int Coords { get; }
        public Vector2Int Direction { get; }
    }
}
