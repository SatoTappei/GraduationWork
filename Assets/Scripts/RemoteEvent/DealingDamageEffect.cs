using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DealingDamageEffect : MonoBehaviour
    {
        [SerializeField] Transform _forwardAxis;
        [SerializeField] Transform _turret;
        [SerializeField] Transform _shell;
        [SerializeField] Transform _muzzle;
        [SerializeField] ParticleSystem _smokeParticle;
        [SerializeField] ParticleSystem _explosionParticle;
        [SerializeField] ParticleSystem _trailParticle;
        [SerializeField] AudioClip _fireSE;
        [SerializeField] AudioClip _shellExplosionSE;
        [SerializeField] AudioClip _smokeEffectSE;
        [SerializeField] MeshRenderer[] _tankRenderers;
        [SerializeField] MeshRenderer _shellRenderer;

        WaitForSeconds _waitExplosionEffect;
        WaitForSeconds _waitExitEffect;

        void OnEnable()
        {
            _shell.localPosition = Vector3.zero;
            _shellRenderer.enabled = true;
            foreach (MeshRenderer r in _tankRenderers) r.enabled = true;
        }

        public void Play(Vector3 position, Adventurer target)
        {
            // �����_���Ȋp�x�Ŕz�u����B
            transform.position = position;
            _forwardAxis.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            
            StartCoroutine(FireAsync(target));
        }

        IEnumerator FireAsync(Adventurer target)
        {
            PlaySE(_smokeEffectSE);
            _smokeParticle.Play();

            Vector3 targetPosition = target.transform.position;
            yield return AimAsync(targetPosition);

            // �e�𔭎ˁB
            PlaySE(_fireSE);
            _trailParticle.Play();

            yield return ShellFlyingAsync(targetPosition);

            // �e�����e���Ĕ����B
            PlaySE(_shellExplosionSE);
            _explosionParticle.Play();
            _shellRenderer.enabled = false;

            if (target != null)
            {
                target.Damage(33, target.Coords); // �_���[�W�ʂ͓K���B
            }

            // �����̉��o���I���܂ő҂B���o�̒����ɍ��킹�Ď��Ԃ��w��B
            yield return _waitExplosionEffect ??= new WaitForSeconds(1.0f);

            // ���o���Đ����đޏ�B
            _smokeParticle.Play();
            foreach (MeshRenderer r in _tankRenderers) r.enabled = false;

            // �ޏꎞ�̉��o���I���܂ő҂B���o�̒����ɍ��킹�Ď��Ԃ��w��B
            yield return _waitExitEffect ??= new WaitForSeconds(1.5f);

            // �v�[���ɖ߂��B
            gameObject.SetActive(false);
        }

        IEnumerator AimAsync(Vector3 targetPosition)
        {
            const float Speed = 1.0f;

            Vector3 start = _turret.forward;
            Vector3 goal = targetPosition - transform.position;
            for (float t = 0; t <= 1; t += Time.deltaTime * Speed)
            {
                _turret.forward = Vector3.Lerp(start, goal, t);
                yield return null;
            }

            _turret.forward = goal;
        }

        IEnumerator ShellFlyingAsync(Vector3 targetPosition)
        {
            const float Speed = 2.0f;

            Vector3 start = _muzzle.position;
            Vector3 goal = targetPosition;
            Vector3 prevPosition = _muzzle.position;
            for (float t = 0; t <= 1; t += Time.deltaTime * Speed)
            {
                // �d�͂ŗ������Ă��銴���o���B
                float x = Vector3.Lerp(start, goal, t).x;
                float y = Vector3.Lerp(start, goal, Easing(t)).y;
                float z = Vector3.Lerp(start, goal, t).z;
                _shell.position = new Vector3(x, y, z);

                Vector3 fwd = _shell.position - prevPosition;
                if (fwd != Vector3.zero) _shell.forward = fwd;

                yield return null;
            }

            _shell.position = goal;
        }

        void PlaySE(AudioClip clip)
        {
            TryGetComponent(out AudioSource audioSource);
            audioSource.clip = clip;
            audioSource.Play();
        }

        static float Easing(float t)
        {
            return t * t * t;
        }
    }
}
