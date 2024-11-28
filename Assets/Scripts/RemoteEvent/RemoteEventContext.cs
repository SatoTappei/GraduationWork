using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RemoteEventContext
    {
        public RemoteEventContext(string id, string coords, string target)
        {
            ID = id;
            Coords = coords;
            Target = target;
        }

        public string ID { get; }
        public string Coords { get; }
        public string Target { get; }
    }
}