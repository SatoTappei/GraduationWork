using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ActionResult
    {
        public ActionResult(string action, string result, string log, 
            Vector2Int coords, Vector2Int direction, Vector2Int? explored = null)
        {
            Action = action;
            Result = result;
            Log = log;
            Coords = coords;
            Direction = direction;
            Explored = explored;
        }

        // 選択した行動とその結果。
        public string Action { get; }
        public string Result { get; }

        // 行動ログに追加する文章。
        public string Log { get; }

        // 行動後の座標と向き。
        public Vector2Int Coords { get; }
        public Vector2Int Direction { get; }

        // 探索した座標。
        public Vector2Int? Explored { get; }
    }
}
