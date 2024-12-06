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
