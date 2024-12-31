using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class NameTag : MonoBehaviour
    {
        [SerializeField] Transform _follow;
        [SerializeField] Vector3 _offset;

        Text _text;
        RectTransform _textTransform;

        void Awake()
        {
            _text = GetComponentInChildren<Text>();
            _textTransform = _text.transform as RectTransform;
        }

        void Update()
        {
            Vector3 position = RectTransformUtility.WorldToScreenPoint(Camera.main, _follow.position);
            _textTransform.position = position + _offset;
        }

        public void SetName(string name)
        {
            _text.text = name;
        }
    }
}
