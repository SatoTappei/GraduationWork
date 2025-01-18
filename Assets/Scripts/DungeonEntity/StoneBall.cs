using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class StoneBall : DungeonEntity, ILeverControllable
    {
        [SerializeField] Transform _parent;
        [SerializeField] Transform _child;
        [SerializeField] Transform _shadow;
        [SerializeField] Renderer _renderer;
        [SerializeField] Renderer _shadowRenderer;
        [SerializeField] ParticleSystem _smokeParticle;
        [SerializeField] ParticleSystem _dustParticle;
        [SerializeField] ParticleSystem _explosionParticle;

        Vector2Int _direction;
        bool _isPlaying;

        void Awake()
        {
            _renderer.enabled = false;
            _shadowRenderer.enabled = false;
        }

        void Start()
        {
            List<Vector2Int> candidate = new List<Vector2Int>()
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            // 上下左右のうち、一番空間が大きな方向に転がる。
            _direction = candidate
                .Select(d => new { Direction = d, Distance = GetDistance(d) })
                .OrderByDescending(x => x.Distance)
                .First()
                .Direction;

            Play(); // テスト
        }

        public void Play()
        {
            if (_isPlaying) return;

            StartCoroutine(PlayAsync());
        }

        IEnumerator PlayAsync()
        {
            _isPlaying = true;
            
            _renderer.enabled = true;
            _shadowRenderer.enabled = true;
            _smokeParticle.Play();

            yield return FallAsync();

            StartCoroutine(RotateAsync());

            _dustParticle.Play();
            yield return TranslateAsync();

            _explosionParticle.Play();
            _renderer.enabled = false;
            _shadowRenderer.enabled = false;

            _isPlaying = false;
        }

        IEnumerator FallAsync()
        {
            Vector3 start = new Vector3(0, 1.0f, 0);
            Vector3 goal = new Vector3(0, -0.09f, 0);
            for (float t = 0; t <= 1.0f; t += Time.deltaTime)
            {
                _parent.localPosition = Vector3.Lerp(start, goal, EasingFall(t));
                yield return null;
            }

            _parent.localPosition = goal;
        }

        IEnumerator TranslateAsync()
        {
            const float Speed = 2.0f;

            int distance = GetDistance(_direction);
            Vector2Int goalCoords = Coords + _direction * distance;
            Vector3 start = _parent.position;
            Vector3 goal = DungeonManager.GetCell(goalCoords).Position;
            for (float t = 0; t <= 1.0f; t += Time.deltaTime / distance * Speed)
            {
                _parent.position = Vector3.Lerp(start, goal, EasingRoll(t));

                Vector3 p = Vector3.Lerp(start, goal, EasingRoll(t));
                p.y = _shadow.position.y;
                _shadow.position = p;

                yield return null;
            }

            _parent.position = goal;
            _shadow.position = goal;
        }

        IEnumerator RotateAsync()
        {
            Vector3 prevPosition = _child.position;
            while (true)
            {
                // 上下左右のうち、マップの北が下、南が上に対応しているのでz方向だけ逆にする必要がある。
                Vector3 diff = prevPosition - _child.position;
                _child.Rotate(new Vector3(-diff.z, 0, diff.x) * 100.0f);

                prevPosition = _child.position;

                yield return null;
            }
        }

        int GetDistance(Vector2Int direction)
        {
            const int Max = 100; // 適当に最大距離を指定。

            for (int i = 0; i <= Max; i++)
            {
                Vector2Int coords = Coords + direction * i;
                Cell cell = DungeonManager.GetCell(coords);

                if (cell.IsImpassable()) return i - 1;
            }

            return Max;
        }

        static float EasingFall(float t)
        {
            return t * t * t * t * t;
        }

        static float EasingRoll(float t)
        {
            return t * t * t;
        }
    }
}

// 次、繰り返し呼び出し、ぶつかったら消える。ぶつかったらダメージを作る。