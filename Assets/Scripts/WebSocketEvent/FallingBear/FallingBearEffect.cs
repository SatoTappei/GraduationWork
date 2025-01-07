using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FallingBearEffect : MonoBehaviour
    {
        [SerializeField] Transform _bear;
        [SerializeField] ParticleSystem _smokeParticle;
        [SerializeField] ParticleSystem _groundHitParticle;

        AudioSource _audioSource;
        WaitForSeconds _waitGroundHit;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(Vector3 position, Adventurer target)
        {
            transform.position = position;

            StartCoroutine(PlayAsync(target));
        }

        IEnumerator PlayAsync(Adventurer target)
        {
            _smokeParticle.Play();
            _audioSource.Play();

            // �����B
            yield return FallAsync();

            // �n�ʂɂԂ���Ɠ����ɒׂ��B
            StartCoroutine(PlayStampAnimationAsync());

            // �n�ʂɂԂ������ۂ̏Ռ��g���o�B
            _groundHitParticle.transform.position = _bear.position;
            _groundHitParticle.Play();

            if (target != null && target.TryGetComponent(out IDamageable damage))
            {
                damage.Damage(1, target.Coords); // �_���[�W�ʂ͓K���B
            }

            // �n�ʂɂԂ������ۂ̉��o���I���܂ő҂B���o�̒����ɍ��킹�Ď��Ԃ��w��B
            yield return _waitGroundHit ??= new WaitForSeconds(1.0f);

            // �v�[���ɖ߂��B
            gameObject.SetActive(false);
        }

        IEnumerator FallAsync()
        {
            const float Speed = 1.0f;

            Vector3 start = Vector3.zero;
            Vector3 goal = Vector3.down * 3.2f; // �n�ʂɐڐG���鍂����K���ɐݒ�B
            for (float t = 0; t <= 1; t += Time.deltaTime * Speed)
            {
                _bear.localPosition = Vector3.Lerp(start, goal, Easing(t));
                yield return null;
            }

            _bear.localPosition = goal;
        }

        IEnumerator PlayStampAnimationAsync()
        {
            Vector3 baseScale = _bear.localScale;
            Vector3 stampedScale = new Vector3(baseScale.x, 0.2f, baseScale.z); // �ׂ���K���ɐݒ�B
            yield return ScaleAnimationAsync(baseScale, stampedScale);
            yield return ScaleAnimationAsync(stampedScale, baseScale);
        }

        IEnumerator ScaleAnimationAsync(Vector3 from, Vector3 to)
        {
            const float Speed = 6.0f;

            for (float t = 0; t <= 1.0f; t += Time.deltaTime * Speed)
            {
                _bear.localScale = Vector3.Lerp(from, to, t);
                yield return null;
            }

            _bear.localScale = to;
        }

        static float Easing(float t)
        {
            return t * t * t * t;
        }
    }
}
