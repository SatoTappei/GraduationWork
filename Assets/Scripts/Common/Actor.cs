using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Actor : MonoBehaviour
    {
        string _id = string.Empty;
        public string ID
        {
            get
            {
                if (_id == string.Empty) _id = GetType().Name;
                return _id;
            }
        }

        public abstract Vector2Int Coords { get; }
        public abstract Vector2Int Direction { get; }
    }
}