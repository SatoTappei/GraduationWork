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

        // �I�������s���Ƃ��̌��ʁB
        public string Action { get; }
        public string Result { get; }

        // �s�����O�ɒǉ����镶�́B
        public string Log { get; }

        // �s����̍��W�ƌ����B
        public Vector2Int Coords { get; }
        public Vector2Int Direction { get; }

        // �T���������W�B
        public Vector2Int? Explored { get; }
    }
}
