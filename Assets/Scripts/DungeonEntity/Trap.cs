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
            // �v�[��������o���ꂽ�^�C�~���O�B���o�t���ŉ�ʂɕ\�������B
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
                damage.Damage(25, Coords); // �_���[�W�ʂ�K���ɐݒ�B
            }

            // ������яo���A�j���[�V�����̍Đ�������҂B�A�j���[�V�����̒����ɍ��킹�Ď��Ԃ��w��B
            yield return _waitTrapAnimation ??= new WaitForSeconds(1.5f);

            // ���o���Đ����đޏ�B
            _smokeParticle.Play();
            foreach (MeshRenderer r in _trapRenderers) r.enabled = false;

            // �ޏꎞ�̉��o���I���܂ő҂B���o�̒����ɍ��킹�Ď��Ԃ��w��B
            yield return _waitExitEffect ??= new WaitForSeconds(1.5f);

            DungeonManager.RemoveActor(Coords, this);

            // �v�[���ɖ߂��B
            gameObject.SetActive(false);
        }
    }
}
