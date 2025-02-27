using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DamageEffect : MonoBehaviour
    {
        [SerializeField] AudioClip _damageSE;
        [SerializeField] ParticleSystem _particle;
        [SerializeField] Blood _blood;

        Actor _actor;
        AudioSource _audio;
        Transform _fbx;
        bool _isKnockback;

        void Awake()
        {
            _actor = GetComponent<Actor>();
            _audio = GetComponent<AudioSource>();
            _fbx = transform.FindChildRecursive("FBX");
        }

        public void Play(Vector2Int coords)
        {
            if (_isKnockback) return;

            StartCoroutine(PlayAsync(_actor.Coords, coords));
        }

        IEnumerator PlayAsync(Vector2Int myselfCoords, Vector2Int attackerCoords)
        {
            _isKnockback = true;

            // SEを再生。
            if (!(_audio == null || _damageSE == null))
            {
                _audio.clip = _damageSE;
                _audio.Play();
            }

            // パーティクルを再生。
            if (_particle != null)
            {
                _particle.Play();
            }

            // 一定確率で血糊を生成。
            if (_blood != null && Random.value < 0.5f) // 確率は適当に指定。
            {
                Blood b = Instantiate(_blood);
                b.transform.position = DungeonManager.GetCell(_actor.Coords).Position;
            }

            // ノックバック。
            Vector2 diff = myselfCoords - attackerCoords;
            Vector3 direction = new Vector3(diff.x, 0, diff.y).normalized;
            yield return KnockbackAsync(-direction);
            yield return KnockbackAsync(direction);

            _fbx.localPosition = Vector3.zero;

            _isKnockback = false;
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
