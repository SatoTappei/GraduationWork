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

        Animator _animator;
        AudioSource _audioSource;
        WaitForSeconds _waitTrapAnimation;
        WaitForSeconds _waitExitEffect;

        void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            // プールから取り出されたタイミング。演出付きで画面に表示される。
            _smokeParticle.Play();
            foreach (MeshRenderer r in _trapRenderers) r.enabled = true;

            _animator.Play("Close Trap");
            _audioSource.clip = _smokeEffectSE;
            _audioSource.Play();
        }

        public override void Interact(Actor user)
        {
            StartCoroutine(InteractAsync(user));
        }

        IEnumerator InteractAsync(Actor user)
        {
            _animator.Play("Open Trap");
            _audioSource.clip = _trapSE;
            _audioSource.Play();

            if (user != null && user.TryGetComponent(out IDamageable damage))
            {
                damage.Damage(25, Coords); // ダメージ量を適当に設定。
            }

            // 槍が飛び出すアニメーションの再生完了を待つ。アニメーションの長さに合わせて時間を指定。
            yield return _waitTrapAnimation ??= new WaitForSeconds(1.5f);

            // 演出を再生して退場。
            _smokeParticle.Play();
            foreach (MeshRenderer r in _trapRenderers) r.enabled = false;

            // 退場時の演出が終わるまで待つ。演出の長さに合わせて時間を指定。
            yield return _waitExitEffect ??= new WaitForSeconds(1.5f);

            DungeonManager.RemoveActor(Coords, this);

            // プールに戻す。
            gameObject.SetActive(false);
        }
    }
}
