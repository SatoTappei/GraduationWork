using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    /// <summary>
    /// 適当な空のオブジェクトにアタッチして使う。
    /// 再生中に消費したトークン数をインスペクター上に表示する。
    /// </summary>
    public class TokenCalculator : MonoBehaviour
    {
        static TokenCalculator _instance;

        [SerializeField] float _total;

        void Start()
        {
            if (_instance == null) _instance = this;
            else Destroy(this);
        }

        void OnDestroy()
        {
            _instance = null;
            _total = 0;
        }

        public static void Add(Response response)
        {
            if (_instance == null) return;

            // GPT4-o-miniの料金。
            float prompt = response.usage.prompt_tokens * 0.15f / 1000000;
            float completion = response.usage.completion_tokens * 0.60f / 1000000;

            _instance._total += prompt + completion;
        }
    }
}
