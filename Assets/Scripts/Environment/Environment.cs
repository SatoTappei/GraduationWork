using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Environment : MonoBehaviour
    {
        public void Initialize(string[] blueprint, int x, int y)
        {
            OnInitialized(blueprint, x, y);
        }

        protected virtual void OnInitialized(string[] blueprint, int x, int y) { }
    }
}
