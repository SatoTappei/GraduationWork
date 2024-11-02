using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class AIRequestFactory
    {
        const string Key = "";

        public static AIRequest Create(string content, int capacity = 7)
        {
            return new AIRequest(Key, content, capacity);
        }
    }
}
