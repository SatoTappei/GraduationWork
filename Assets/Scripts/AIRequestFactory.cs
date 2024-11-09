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
