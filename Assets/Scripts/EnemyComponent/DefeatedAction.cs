using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Game.EnemyComponent
{
    public class DefeatedAction : BaseAction
    {
        [SerializeField] AudioClip _defeatedSE;
        [SerializeField] ParticleSystem _particle;
        [SerializeField] Renderer[] _renderers;
        [SerializeField] DungeonEntity[] _dropItems;

        Enemy _enemy;
        Animator _animator;
        AudioSource _audioSource;

        void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _animator = GetComponentInChildren<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }

        public async UniTask<bool> PlayAsync(CancellationToken token)
        {
            if (_enemy.Status.IsAlive) return false;

            _animator.applyRootMotion = true;
            _animator.Play("Death");
            
            _audioSource.clip = _defeatedSE;
            _audioSource.Play();

            // ランダムなアイテムをドロップ
            if (_dropItems != null && 0 < _dropItems.Length)
            {
                int random = Random.Range(0, _dropItems.Length);
                DungeonEntity item = Instantiate(_dropItems[random]);
                item.Place(_enemy.Coords, Vector2Int.up);
            }

            // 死亡アニメーションの再生終了後にパーティクルを出して画面から消える。時間は適当に指定。
            await UniTask.WaitForSeconds(1.5f, cancellationToken: token);

            _animator.applyRootMotion = false;

            _particle.Play();

            foreach (Renderer r in _renderers)
            {
                r.enabled = false;
            }

            // パーティクルの再生終了を待つ。時間は適当に指定。
            await UniTask.WaitForSeconds(1.0f, cancellationToken: token);

            return true;
        }
    }
}
