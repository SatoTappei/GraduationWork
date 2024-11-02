using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class BilingualString
    {
        [SerializeField] string _japanese;
        [SerializeField] string _english;

        public BilingualString(string japanese, string english)
        {
            _japanese = japanese;
            _english = english;
        }

        public string Japanese => _japanese;
        public string English => _english;
    }
}
