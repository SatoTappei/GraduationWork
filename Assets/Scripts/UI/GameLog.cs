using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameLog : MonoBehaviour
    {
        struct LogContent
        {
            public string Label;
            public string Value;
            public GameLogColor Color;
        }

        [SerializeField] GameLogUI[] _rows;
        [SerializeField] Vector3 _position;
        [SerializeField] float _height = 120.0f;

        Queue<GameLogUI> _pool;
        List<GameLogUI> _used;
        Queue<LogContent> _buffer;

        bool _isAnimationPlaying;

        void Awake()
        {
            _pool = new Queue<GameLogUI>();
            _used = new List<GameLogUI>();
            _buffer = new Queue<LogContent>();

            foreach (GameLogUI ui in _rows)
            {
                _pool.Enqueue(ui);
                ui.Set(string.Empty, string.Empty);
            }
        }

        void Start()
        {
            StartCoroutine(PlayAnimationRepeatingAsync());
            StartCoroutine(DisplayBufferdRepeatingAsync());
        }

        public static bool TryFind(out GameLog result)
        {
            result = GameObject.FindGameObjectWithTag("UiManager").GetComponent<GameLog>();
            return result != null;
        }

        public void Add(string label, string value, GameLogColor color)
        {
            _buffer.Enqueue(new LogContent() { Label = label, Value = value, Color = color });
        }

        IEnumerator DisplayBufferdRepeatingAsync()
        {
            WaitForSeconds waitInterval = null;
            while (true)
            {
                if (_buffer.TryPeek(out LogContent content) && TryDisplay(content)) _buffer.Dequeue();

                // 毎フレームバッファをチェックせずとも十分。時間は適当に指定。
                yield return waitInterval ??= new WaitForSeconds(0.1f);
            }
        }

        bool TryDisplay(LogContent content)
        {
            if (_isAnimationPlaying) return false;

            if (_pool.TryDequeue(out GameLogUI ui))
            {
                ui.transform.localPosition = _position + Vector3.up * _height * _used.Count;
                ui.Set(content.Label, content.Value, content.Color);

                _used.Add(ui);

                return true;
            }

            return false;
        }

        IEnumerator PlayAnimationRepeatingAsync()
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
                coroutine = StartCoroutine(LocalTranslateAsync(_used[i].transform));
            }

            yield return coroutine;

            // プールに戻す。
            GameLogUI ui = _used[0];
            _used.RemoveAt(0);
            _pool.Enqueue(ui);
        }

        IEnumerator LocalTranslateAsync(Transform transform)
        {
            const float Speed = 1.0f;

            Vector3 start = transform.localPosition;
            Vector3 goal = start + Vector3.down * _height;
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
    }
}