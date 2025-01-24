using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class NameTagUI : MonoBehaviour
    {
        class Data
        {
            public Transform Follow;
            public RectTransform Tag;
        }

        [SerializeField] Camera _camera;
        [SerializeField] Text _textPrefab;
        [SerializeField] Vector3 _offset;
        [SerializeField] Vector2 _buttonLeft = new Vector2(20.0f, -520.0f);
        [SerializeField] Vector2 _topRight = new Vector2(940.0f, -20.0f);

        Dictionary<int, Data> _registered;

        void Awake()
        {
            _registered = new Dictionary<int, Data>();
        }

        void Update()
        {
            foreach (Data data in _registered.Values)
            {
                Vector3 p = RectTransformUtility.WorldToScreenPoint(_camera, data.Follow.position);
                data.Tag.position = p + _offset;

                float x = data.Tag.localPosition.x;
                float y = data.Tag.localPosition.y;
                if(_buttonLeft.x < x && x < _topRight.x && _buttonLeft.y < y && y < _topRight.y)
                {
                    data.Tag.localScale = Vector3.one;
                }
                else
                {
                    data.Tag.localScale = Vector3.zero;
                }
            }
        }

        public void Add(Adventurer adventurer)
        {
            Text t = Instantiate(_textPrefab, transform);
            t.text = adventurer.AdventurerSheet.DisplayName;

            _registered.Add(
                adventurer.GetInstanceID(), 
                new Data() { Follow = adventurer.transform, Tag = t.rectTransform }
            );
        }

        public void Remove(Adventurer adventurer)
        {
            var key = adventurer.GetInstanceID();
            if (_registered.ContainsKey(key))
            {
                Destroy(_registered[key].Tag.gameObject);

                _registered.Remove(key);
            }
            else
            {
                Debug.Log($"Šù‚ÉíœÏ‚ÝB{adventurer.AdventurerSheet.DisplayName}");
            }
        }
    }
}
