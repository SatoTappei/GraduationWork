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
                // �_���[�W�ʂ�K���ɐݒ�B
                damage.Damage(nameof(Trap), nameof(Trap), 25, Coords);
            }

            // ������яo���A�j���[�V�����̍Đ�������҂B�A�j���[�V�����̒����ɍ��킹�Ď��Ԃ��w��B
            yield return _waitTrapAnimation ??= new WaitForSeconds(1.5f);

            // ���o���Đ����đޏ�B
            _smokeParticle.Play();
            foreach (MeshRenderer r in _trapRenderers) r.enabled = false;

            // �ޏꎞ�̉��o���I���܂ő҂B���o�̒����ɍ��킹�Ď��Ԃ��w��B
            yield return _waitExitEffect ??= new WaitForSeconds(1.5f);

            DungeonManager.RemoveActorOnCell(Coords, this);

            // �v�[���ɖ߂��B
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
