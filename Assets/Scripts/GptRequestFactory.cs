using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class GptRequestFactory
    {
        const string Key = "";

        public static GptRequest Create(string content, int capacity = 7)
        {
            return new GptRequest(Key, content, capacity);
        }
    }
}
