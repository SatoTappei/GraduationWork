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
        [SerializeField] string _name;

        Text _text;
        RectTransform _textTransform;

        void Awake()
        {
            _text = GetComponentInChildren<Text>();
            _textTransform = _text.transform as RectTransform;
        }

        void Start()
        {
            SetName(_name);
            UpdatePosition();
        }

        void Update()
        {
            UpdatePosition();
        }

        public void SetName(string name)
        {
            _text.text = name;
        }

        void UpdatePosition()
        {
            Vector3 position = RectTransformUtility.WorldToScreenPoint(Camera.main, _follow.position);
            _textTransform.position = position + _offset;
        }
    }
}
