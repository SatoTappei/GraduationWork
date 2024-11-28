using System.Collections;
using UnityEngine;

namespace Game
{
    public class Trap : DungeonEntity
    {
        [SerializeField] ParticleSystem _smokeParticle;
        [SerializeField] MeshRenderer[] _trapRenderers;

        WaitForSeconds _waitTrapAnimation;
        WaitForSeconds _waitExitEffect;

        void OnEnable()
        {
            _smokeParticle.Play();
            foreach (MeshRenderer r in _trapRenderers) r.enabled = true;

            this.TryGetComponentInChildren(out Animator animator);
            animator.Play("Close Trap");
        }

        public override void Interact(Actor user)
        {
            StartCoroutine(InteractAsync(user));
        }

        IEnumerator InteractAsync(Actor user)
        {
            this.TryGetComponentInChildren(out Animator animator);
            animator.Play("Open Trap");

            TryGetComponent(out AudioSource audio);
            audio.Play();

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

            DungeonManager dungeonManager = DungeonManager.Find();
            dungeonManager.RemoveActorOnCell(Coords, this);

            // �v�[���ɖ߂��B
            gameObject.SetActive(false);
        }
    }
}
