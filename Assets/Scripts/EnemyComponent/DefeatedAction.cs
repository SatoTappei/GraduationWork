using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game.EnemyComponent
{
    public class DefeatedAction : BaseAction
    {
        [SerializeField] AudioClip _defeatedSE;
        [SerializeField] ParticleSystem _particle;
        [SerializeField] Renderer[] _renderers;

        Enemy _enemy;
        Animator _animator;
        AudioSource _audioSource;

        void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }

        public async UniTask<bool> PlayAsync(CancellationToken token)
        {
            if (_enemy.Status.IsAlive) return false;

            _animator.applyRootMotion = true;
            _animator.Play("Death");
            
            _audioSource.clip = _defeatedSE;
            _audioSource.Play();

            // ���S�A�j���[�V�����̍Đ��I����Ƀp�[�e�B�N�����o���ĉ�ʂ��������B���Ԃ͓K���Ɏw��B
            await UniTask.WaitForSeconds(1.5f, cancellationToken: token);

            _animator.applyRootMotion = false;

            _particle.Play();

            foreach (Renderer r in _renderers)
            {
                r.enabled = false;
            }

            // �p�[�e�B�N���̍Đ��I����҂B���Ԃ͓K���Ɏw��B
            await UniTask.WaitForSeconds(1.0f, cancellationToken: token);

            return true;
        }
    }
}
