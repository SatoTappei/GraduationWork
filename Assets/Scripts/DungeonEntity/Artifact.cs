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

        // 条件を満たすことで出現するので、初期状態では漁れないようフラグを立てておく。
        public bool IsEmpty { get; private set; } = true;

        void Start()
        {
            // ボスの生成座標を手動で指定。
            _spawner = DungeonManager
                .GetActors(new Vector2Int(17, 19))
                .Select(a => a as EnemySpawner)
                .FirstOrDefault();

            // ボスが死亡したタイミングでアーティファクトが湧くように登録。
            if (_spawner == null)
            {
                Debug.LogWarning("ボスのスポナーがnullになっている。");
            }
            else
            {
                _spawner.OnDefeated += Refill;
            }

            // 条件を満たすまで画面に映らないようにしておく。
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

            // 条件を満たすまで画面に映らないようにしておく。
            _renderer.enabled = false;
            foreach (ParticleSystem p in _particles)
            {
                p.gameObject.SetActive(false);
            }

            // モデルを回転させる。
            if (_rotate != null) StopCoroutine(_rotate);

            return new Item("★アーティファクト", "Artifact");
        }

        void Refill()
        {
            if (!IsEmpty) return;

            // 画面に表示させる。
            _renderer.enabled = true;
            foreach (ParticleSystem p in _particles)
            {
                p.gameObject.SetActive(true);
            }

            // モデルを回転させる。
            _rotate = StartCoroutine(RotateAsync());

            IsEmpty = false;
        }

        IEnumerator RotateAsync()
        {
            Vector3 basePosition = _fbx.localPosition;
            while (true)
            {
                // アイテムが回転しつつ浮遊している感じの動き。値は適当。
                _fbx.Rotate(Vector3.up * Time.deltaTime * 20.0f);
                _fbx.localPosition = basePosition + Vector3.up * Mathf.Sin(Time.time) * 0.2f;

                yield return null;
            }
        }
    }
}
