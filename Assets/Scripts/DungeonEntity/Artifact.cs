using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class Artifact : DungeonEntity, IScavengeable
    {
        [SerializeField] Transform _fbx;
        [SerializeField] Renderer _renderer;
        [SerializeField] ParticleSystem[] _particles;

        EnemySpawner _spawner;
        Coroutine _rotate;

        // �����𖞂������Ƃŏo������̂ŁA������Ԃł͋���Ȃ��悤�t���O�𗧂ĂĂ����B
        public bool IsEmpty { get; private set; } = true;

        void Start()
        {
            // �{�X�̐������W���蓮�Ŏw��B
            _spawner = DungeonManager
                .GetActors(new Vector2Int(17, 19))
                .Select(a => a as EnemySpawner)
                .FirstOrDefault();

            // �{�X�����S�����^�C�~���O�ŃA�[�e�B�t�@�N�g���N���悤�ɓo�^�B
            if (_spawner == null)
            {
                Debug.LogWarning("�{�X�̃X�|�i�[��null�ɂȂ��Ă���B");
            }
            else
            {
                _spawner.OnDefeated += Refill;
            }

            // �����𖞂����܂ŉ�ʂɉf��Ȃ��悤�ɂ��Ă����B
            _renderer.enabled = false;
            foreach (ParticleSystem p in _particles)
            {
                p.gameObject.SetActive(false);
            }
        }

        void OnDisable()
        {
            if (_spawner != null) _spawner.OnDefeated -= Refill;
        }

        public Item Scavenge()
        {
            if (IsEmpty) return null;

            IsEmpty = true;

            // �����𖞂����܂ŉ�ʂɉf��Ȃ��悤�ɂ��Ă����B
            _renderer.enabled = false;
            foreach (ParticleSystem p in _particles)
            {
                p.gameObject.SetActive(false);
            }

            // ���f������]������B
            if (_rotate != null) StopCoroutine(_rotate);

            return new Item("���A�[�e�B�t�@�N�g", "Artifact");
        }

        void Refill()
        {
            if (!IsEmpty) return;

            // ��ʂɕ\��������B
            _renderer.enabled = true;
            foreach (ParticleSystem p in _particles)
            {
                p.gameObject.SetActive(true);
            }

            // ���f������]������B
            _rotate = StartCoroutine(RotateAsync());

            IsEmpty = false;
        }

        IEnumerator RotateAsync()
        {
            Vector3 basePosition = _fbx.localPosition;
            while (true)
            {
                // �A�C�e������]�����V���Ă��銴���̓����B�l�͓K���B
                _fbx.Rotate(Vector3.up * Time.deltaTime * 20.0f);
                _fbx.localPosition = basePosition + Vector3.up * Mathf.Sin(Time.time) * 0.2f;

                yield return null;
            }
        }
    }
}
