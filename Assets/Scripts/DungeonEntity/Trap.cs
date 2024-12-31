using System.Collections;
using UnityEngine;

namespace Game
{
    public class Trap : DungeonEntity
    {
        [SerializeField] ParticleSystem _smokeParticle;
        [SerializeField] MeshRenderer[] _trapRenderers;
        [SerializeField] AudioClip _smokeEffectSE;
        [SerializeField] AudioClip _trapSE;

        WaitForSeconds _waitTrapAnimation;
        WaitForSeconds _waitExitEffect;

        void OnEnable()
        {
            _smokeParticle.Play();
            foreach (MeshRenderer r in _trapRenderers) r.enabled = true;

            this.TryGetComponentInChildren(out Animator animator);
            animator.Play("Close Trap");
            PlaySE(_smokeEffectSE);
        }

        public override void Interact(Actor user)
        {
            StartCoroutine(InteractAsync(user));
        }

        IEnumerator InteractAsync(Actor user)
        {
            this.TryGetComponentInChildren(out Animator animator);
            animator.Play("Open Trap");
            PlaySE(_trapSE);

            if (user != null && user.TryGetComponent(out IDamageable damage))
            {
                // ダメージ量を適当に設定。
                damage.Damage(nameof(Trap), nameof(Trap), 25, Coords);
            }

            // 槍が飛び出すアニメーションの再生完了を待つ。アニメーションの長さに合わせて時間を指定。
            yield return _waitTrapAnimation ??= new WaitForSeconds(1.5f);

            // 演出を再生して退場。
            _smokeParticle.Play();
            foreach (MeshRenderer r in _trapRenderers) r.enabled = false;

            // 退場時の演出が終わるまで待つ。演出の長さに合わせて時間を指定。
            yield return _waitExitEffect ??= new WaitForSeconds(1.5f);

            DungeonManager.RemoveActorOnCell(Coords, this);

            // プールに戻す。
            gameObject.SetActive(false);
        }

        void PlaySE(AudioClip clip)
        {
            TryGetComponent(out AudioSource audio);
            audio.clip = clip;
            audio.Play();
        }
    }
}
