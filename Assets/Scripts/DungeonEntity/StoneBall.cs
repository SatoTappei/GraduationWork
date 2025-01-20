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

            // ���o��҂B���Ԃ͓K���Ɏw��B
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

            // �㉺���E�̂����A��ԋ�Ԃ��傫�ȕ����ɓ]����B
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

                // �ʂ̃Z���Ɉړ������ꍇ�A���̃Z���ɂ���ΏۂɃ_���[�W��^����B
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
                // �㉺���E�̂����A�}�b�v�̖k�����A�삪��ɑΉ����Ă���̂�z���������t�ɂ���K�v������B
                Vector3 diff = prevPosition - _child.position;
                _child.Rotate(new Vector3(-diff.z, 0, diff.x) * 100.0f);

                prevPosition = _child.position;

                yield return null;
            }
        }

        int GetDistance(Vector2Int direction)
        {
            const int Max = 100; // �K���ɍő勗�����w��B

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

            // �����Ȃ��A�N�����Ȃ��ꍇ�B
            if (actors.Count == 0) return false;

            bool isCrash = false;
            foreach (Actor actor in DungeonManager.GetActors(coords))
            {
                // �`���҂܂��͓G������ꍇ�̓_���[�W��^����B
                if (actor.TryGetComponent(out IDamageable damage))
                {
                    damage.Damage(70, coords); // �_���[�W�ʂ͓K���B
                }

                // DungeonEntity�Ŕ��肷��ƁA�����̖��@�w�ɂԂ������ꍇ�ł��j�󂳂�Ă��܂��B
                // ����̔z�u���Ɛi�H��ɂ����Q���̓h�A�݂̂Ȃ̂ŁA�s���|�C���g�Ŕ��肷��Ώ\���B
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