using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class CheerCommentUI : MonoBehaviour
    {
        [SerializeField] Text _text;

        WaitForSeconds _wait;

        void Awake()
        {
            // プールに戻す。
            gameObject.SetActive(false);
        }

        public void Play(string comment, int emotion)
        {
            StartCoroutine(PlayAsync(comment, emotion));
        }

        IEnumerator PlayAsync(string comment, int emotion)
        {
            _text.text = comment;

            // 演出付きで表示させる。
            Coroutine fadeIn = StartCoroutine(FadeAsync(0, 1));
            Coroutine moveIn = StartCoroutine(TranslateAsync(Vector2.up * 20, Vector2.zero));
            yield return fadeIn;
            yield return moveIn;

            // 一定時間画面に表示させる。
            yield return _wait ??= new WaitForSeconds(3.0f);

            Coroutine fadeOut = StartCoroutine(FadeAsync(1, 0));
            Coroutine moveOut = StartCoroutine(TranslateAsync(Vector2.zero, Vector2.down * 20));
            yield return fadeOut;
            yield return moveOut;

            // プールに戻す。
            gameObject.SetActive(false);
        }

        IEnumerator FadeAsync(float start, float goal)
        {
            const float Speed = 5.0f;

            for (float t = 0; t <= 1.0f; t += Time.deltaTime * Speed)
            {
                Color c = _text.color;
                c.a = Mathf.Lerp(start, goal, t);
                _text.color = c;

                yield return null;
            }
        }

        IEnumerator TranslateAsync(Vector2 start, Vector2 goal)
        {
            const float Speed = 5.0f;

            Transform text = _text.transform;
            for (float t = 0; t <= 1.0f; t += Time.deltaTime * Speed)
            {
                text.localPosition = Vector2.Lerp(start, goal, Easing(t));
                
                yield return null;
            }

            text.localPosition = goal;
        }

        float Easing(float t)
        {
            return t * t * t;
        }
    }
}
