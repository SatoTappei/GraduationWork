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

        WaitForSeconds _waitInterval;
        bool _isPlaying;

        void Awake()
        {
            _renderer.enabled = false;
            _shadowRenderer.enabled = false;
        }

        public void Play()
        {
            if (_isPlaying) return;

            StartCoroutine(PlayAsync());
        }

        IEnumerator PlayAsync()
        {
            _isPlaying = true;

            Vector3 defaultShadowPosition = _shadow.position;
            _renderer.enabled = true;
            _shadowRenderer.enabled = true;
            _smokeParticle.Play();

            yield return FallAsync();

            Coroutine rotate = StartCoroutine(RotateAsync());
            _dustParticle.Play();

            yield return TranslateAsync();

            _dustParticle.Stop();
            StopCoroutine(rotate);

            _explosionParticle.Play();
            _renderer.enabled = false;
            _shadowRenderer.enabled = false;
            _shadow.position = defaultShadowPosition;

            // 演出を待つ。時間は適当に指定。
            yield return _waitInterval ??= new WaitForSeconds(1.5f);

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

            // 上下左右のうち、一番空間が大きな方向に転がる。
            Vector2Int direction = GetDirections()
                .Select(d => new { Direction = d, Distance = GetDistance(d) })
                .OrderByDescending(x => x.Distance)
                .First()
                .Direction;

            int distance = GetDistance(direction);
            Vector2Int goalCoords = Coords + direction * distance;
            Vector2Int prevCoords = Coords;
            Vector3 start = _parent.position;
            Vector3 goal = DungeonManager.GetCell(goalCoords).Position;
            for (float t = 0; t <= 1.0f; t += Time.deltaTime / distance * Speed)
            {
                Vector3 p = Vector3.Lerp(start, goal, EasingTranslate(t));

                _parent.position = p;
                _shadow.position = new Vector3(p.x, _shadow.position.y, p.z);

                // 別のセルに移動した場合、そのセルにいる対象にダメージを与える。
                Vector2Int currentCoords = DungeonManager.GetCell(p).Coords;
                if (currentCoords != prevCoords && Check(currentCoords)) break;

                prevCoords = currentCoords;

                yield return null;
            }
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

        static bool Check(Vector2Int coords)
        {
            IReadOnlyList<Actor> actors = DungeonManager.GetActors(coords);

            // 何もない、誰もいない場合。
            if (actors.Count == 0) return false;

            bool isCrash = false;
            foreach (Actor actor in DungeonManager.GetActors(coords))
            {
                // 冒険者または敵がいる場合はダメージを与える。
                if (actor.TryGetComponent(out IDamageable damage))
                {
                    damage.Damage(70, coords); // ダメージ量は適当。
                }

                // DungeonEntityで判定すると、入口の魔法陣にぶつかった場合でも破壊されてしまう。
                // 現状の配置だと進路上にある障害物はドアのみなので、ピンポイントで判定すれば十分。
                if (actor.TryGetComponent(out Door _))
                {
                    isCrash = true;
                }
            }

            return isCrash;
        }

        static IEnumerable<Vector2Int> GetDirections()
        {
            yield return Vector2Int.up;
            yield return Vector2Int.down;
            yield return Vector2Int.left;
            yield return Vector2Int.right;
        }

        static float EasingFall(float t)
        {
            return t * t * t * t * t;
        }

        static float EasingTranslate(float t)
        {
            return t * t * t;
        }
    }
}