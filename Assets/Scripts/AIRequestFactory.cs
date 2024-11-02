using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class AIRequestFactory
    {
        const string Key = "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";

        public static AIRequest Create(string content, int capacity = 7)
        {
            return new AIRequest(Key, content, capacity);
        }
    }
}
