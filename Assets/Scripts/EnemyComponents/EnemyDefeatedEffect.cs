using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class EnemyDefeatedEffect : MonoBehaviour
    {
        [SerializeField] AudioClip _defeatedSE;
        [SerializeField] ParticleSystem _particle;
        [SerializeField] Renderer[] _renderers;

        public async UniTask PlayAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            Animator animator = GetComponentInChildren<Animator>();
            AudioSource audioSource = GetComponent<AudioSource>();

            if (animator != null)
            {
                animator.applyRootMotion = true;
                animator.Play("Death");
            }

            if (audioSource != null && _defeatedSE != null)
            {
                audioSource.clip = _defeatedSE;
                audioSource.Play();
            }

            // 死亡アニメーションの再生終了後にパーティクルを出して画面から消える。時間は適当に指定。
            await UniTask.WaitForSeconds(1.5f, cancellationToken: token);

            if (animator != null)
            {
                animator.applyRootMotion = false;
            }

            if (_particle != null)
            {
                _particle.Play();
            }

            foreach (Renderer r in _renderers)
            {
                r.enabled = false;
            }

            // パーティクルの再生終了を待つ。時間は適当に指定。
            await UniTask.WaitForSeconds(1.0f, cancellationToken: token);
        }
    }
}
