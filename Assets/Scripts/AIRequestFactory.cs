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
