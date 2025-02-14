using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Blood : MonoBehaviour
    {
        [SerializeField] SpriteRenderer[] _renderers;

        void Awake()
        {
            foreach (SpriteRenderer r in _renderers)
            {
                r.enabled = false;
            }

            _rendered = _renderers[Random.Range(0, _renderers.Length)];
            _rendered.enabled = true;
        }

        SpriteRenderer _rendered;

        void Start()
        {
            StartCoroutine(PlayAsync());
        }

        IEnumerator PlayAsync()
        {
            yield return new WaitForSeconds(10.0f);

            // èôÅXÇ…âÊñ Ç©ÇÁè¡Ç¶ÇÈÅB
            float a = _rendered.color.a;
            for (float t = 0; t <= 1.0f; t += Time.deltaTime * 0.1f)
            {
                Color color = _rendered.color;
                color.a = Mathf.Lerp(a, 0, t);
                _rendered.color = color;

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
