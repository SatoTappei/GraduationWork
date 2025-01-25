using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameLogUI : MonoBehaviour
    {
        class Content
        {
            public string Label;
            public string Value;
            public LogColor Color;
        }

        [SerializeField] Row[] _rows;
        [SerializeField] Vector2 _position;
        [SerializeField] float _height = 33.0f;

        Queue<Row> _pool;
        List<Row> _used;
        Queue<Content> _buffer;

        bool _isAnimationPlaying;

        void Awake()
        {
            _pool = new Queue<Row>();
            _used = new List<Row>();
            _buffer = new Queue<Content>();

            foreach (Row ui in _rows)
            {
                _pool.Enqueue(ui);
            }
        }

        void Start()
        {
            StartCoroutine(StreamRepeatingAsync());
            StartCoroutine(DisplayRepeatingAsync());
        }

        public void Add(string label, string value, LogColor color)
        {
            _buffer.Enqueue(new Content() { Label = label, Value = value, Color = color });
        }

        IEnumerator StreamRepeatingAsync()
        {
            WaitForSeconds waitInterval = null;
            WaitUntil waitDisplay = null;
            while (true)
            {
                // 1つ以上ログが表示されるまで待つ。
                yield return waitDisplay ??= new WaitUntil(() => _used.Count > 0);

                // 次のログが流れるまで少し待つ。時間は適当に指定。
                yield return waitInterval ??= new WaitForSeconds(2.0f);

                _isAnimationPlaying = true;
                yield return TranslateRowsAsync();
                _isAnimationPlaying = false;
            }
        }

        IEnumerator TranslateRowsAsync()
        {
            if (_used.Count == 0)
            {
                yield return null;
                yield break;
            }

            // 同時に動かすので1つだけ待つ。
            Coroutine coroutine = null;
            for (int i = 0; i < _used.Count; i++)
            {
                coroutine = StartCoroutine(TranslateRowAsync(_used[i].transform));
            }

            yield return coroutine;

            // プールに戻す。
            Row ui = _used[0];
            _used.RemoveAt(0);
            _pool.Enqueue(ui);
        }

        IEnumerator TranslateRowAsync(Transform row)
        {
            const float Speed = 1.0f;

            Vector3 start = row.localPosition;
            Vector3 goal = start + Vector3.down * _height;
            for (float t = 0; t <= 1.0f; t += Time.deltaTime * Speed)
            {
                row.localPosition = Vector3.Lerp(start, goal, Easing(t));
                yield return null;
            }
        }

        IEnumerator DisplayRepeatingAsync()
        {
            WaitForSeconds waitInterval = null;
            while (true)
            {
                if (_buffer.TryPeek(out Content content) && TryDisplay(content))
                {
                    _buffer.Dequeue();
                }

                // 毎フレームバッファをチェックせずとも十分。時間は適当に指定。
                yield return waitInterval ??= new WaitForSeconds(0.1f);
            }
        }

        bool TryDisplay(Content content)
        {
            if (_isAnimationPlaying) return false;

            if (_pool.TryDequeue(out Row ui))
            {
                ui.transform.localPosition = (Vector3)_position + Vector3.up * _height * _used.Count;
                ui.Set(content.Label, content.Value, content.Color);

                _used.Add(ui);

                return true;
            }

            return false;
        }

        static float Easing(float t)
        {
            float x = 1.0f - t;
            return 1 - x * x * x * x * x;
        }
    }
}