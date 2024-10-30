using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IProfileWindowDisplayStatus
    {
        public string FullName { get; }
        public string Job { get; }
        public string Background { get; }
        public string Goal { get; }
        public string[] Items { get; }
        public string[] Infomation { get; }
    }
}
