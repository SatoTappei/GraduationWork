using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DamageEffect : MonoBehaviour
    {
        [SerializeField] AudioClip _damageSE;
        [SerializeField] ParticleSystem _particle;

        AudioSource _audioSource;
        Transform _fbx;
        bool _isKnockback;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _fbx = transform.FindChildRecursive("FBX");
        }

        public void Play(Vector2Int coords, Vector2Int attackerCoords)
        {
            if (_isKnockback) return;

            StartCoroutine(PlayAsync(coords, attackerCoords));
        }

        IEnumerator PlayAsync(Vector2Int coords, Vector2Int attackerCoords)
        {
            _isKnockback = true;

            PlaySE();
            PlayParticle();

            Vector3 forward = CoordsToDirection(coords, attackerCoords);
            yield return KnockbackAsync(-forward);
            yield return KnockbackAsync(forward);

            _fbx.localPosition = Vector3.zero;

            _isKnockback = false;
        }

        void PlaySE()
        {
            if (_audioSource == null || _damageSE == null) return;

            _audioSource.clip = _damageSE;
            _audioSource.Play();
        }

        void PlayParticle()
        {
            if (_particle == null) return;

            _particle.Play();
        }

        static Vector3 CoordsToDirection(Vector2Int a, Vector2Int b)
        {
            Vector2 d = a - b;
            return new Vector3(d.x, 0, d.y).normalized;
        }

        IEnumerator KnockbackAsync(Vector3 direction)
        {
            const float Speed = 10.0f;
            const float Distance = 0.2f;

            Vector3 start = _fbx.position;
            Vector3 goal = start + direction * Distance;
            for (float t = 0; t <= 1; t += Time.deltaTime * Speed)
            {
                _fbx.position = Vector3.Lerp(start, goal, t);
                yield return null;
            }
        }
    }
}
