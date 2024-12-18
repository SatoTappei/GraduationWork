using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class コメント流すやつ : MonoBehaviour
{
    [SerializeField] LogRowUI[] _rows;
    [SerializeField] Vector3 _position;
    [SerializeField] float _height = 60.0f;

    Queue<string> _q;

    void Awake()
    {
        _q = new Queue<string>();
    }

    void Start()
    {
        // ヒエラルキーの並び順でアサインすると、前の要素の方が上にくる。
        for (int i = 1; i >= _rows.Length; i--)
        {
            _rows[^i].transform.localPosition = _position + Vector3.up * _height * i;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 start = _rows[^1].transform.localPosition;
            Vector3 goal = start + Vector3.down * 100.0f;
            StartCoroutine(LocalTranslateAsync(_rows[^1].transform, start, goal));
        }
    }

    static IEnumerator LocalTranslateAsync(Transform transform, Vector3 start, Vector3 goal)
    {
        const float Speed = 1.0f;

        for (float t = 0; t <= 1.0f; t += Time.deltaTime * Speed)
        {
            transform.localPosition = Vector3.Lerp(start, goal, Easing(t));
            yield return null;
        }
    }

    static float Easing(float t)
    {
        float x = 1.0f - t;
        return 1 - x * x * x * x * x;
    }

    void Add(string text)
    {
        _q.Enqueue(text);
    }
}