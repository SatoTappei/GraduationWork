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
        [SerializeField] Vector2 _offset;
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
                // �`���҂̓���ɕ\������UI��Ǐ]������B
                Vector3 position = RectTransformUtility.WorldToScreenPoint(_camera, data.Follow.position);
                data.Tag.position = position + (Vector3)_offset;

                // ��ʂ�4�������Ă���̂ŁA�͈͊O�ɏo���ꍇ�͕\�����Ȃ��悤�ɂ���B
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
            t.text = adventurer.Sheet.DisplayName;

            _registered.Add(
                adventurer.GetInstanceID(), 
                new Data() { Follow = adventurer.transform, Tag = t.rectTransform }
            );
        }

        public void Remove(Adventurer adventurer)
        {
            int key = adventurer.GetInstanceID();
            if (_registered.ContainsKey(key))
            {
                RectTransform tag = _registered[key].Tag;
                if (tag != null) Destroy(tag.gameObject);

                _registered.Remove(key);
            }
            else
            {
                Debug.LogWarning($"���ɍ폜�ς݁B{adventurer.Sheet.DisplayName}");
            }
        }
    }
}
