using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class DisplayedComment : MonoBehaviour
    {
        [SerializeField] float _speed = 0.15f;

        Text _text;

        // 1����������̑傫���́A���ۂɕ\������镶�������Ē����B
        // Text��FontSize��64�̏ꍇ�A55���ǂ������B
        public float TextSize => _text.text.Length * 55.0f;

        void Awake()
        {
            _text = GetComponent<Text>();
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        public void Play()
        {
            StartCoroutine(FlowAsync());
        }

        IEnumerator FlowAsync()
        {
            // ��ʍ���0�Ƃ��āA�\������Ă��镶���̒����Ԃ񍶂̈ʒu�B
            // ��ʂ��當���񂪊��S�ɏ�����܂ŁB
            while (-TextSize < transform.position.x)
            {
                transform.position += Vector3.left * Time.deltaTime * _speed;
                yield return null;
            }

            // �v�[���ɖ߂��B
            gameObject.SetActive(false);
        }
    }
}
