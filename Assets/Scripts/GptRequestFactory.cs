using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GptRequestFactory : MonoBehaviour
    {
        static GptRequestFactory _instance;

        [SerializeField] string _key;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Debug.LogWarning($"{nameof(GptRequestFactory)}のインスタンスが重複している。");
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            if (_instance != null) _instance = null;
        }

        public static GptRequest Create(string content, int capacity = 7)
        {
            return new GptRequest(_instance._key, content, capacity);
        }
    }
}
