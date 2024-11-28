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

        // 1文字当たりの大きさは、実際に表示される文字を見て調整。
        // TextのFontSizeが64の場合、55が良い感じ。
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
            // 画面左を0として、表示されている文字の長さぶん左の位置。
            // 画面から文字列が完全に消えるまで。
            while (-TextSize < transform.position.x)
            {
                transform.position += Vector3.left * Time.deltaTime * _speed;
                yield return null;
            }

            // プールに戻す。
            gameObject.SetActive(false);
        }
    }
}
