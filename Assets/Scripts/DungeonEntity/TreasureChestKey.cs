using Game.ItemData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class TreasureChestKey : DungeonEntity, IScavengeable
    {
        // 現状、軽い鍵 と 重い鍵 は見た目以外に違いが無いので列挙型での判定で十分。
        enum Type { Light, Heavy }

        [SerializeField] Transform _fbx;
        [SerializeField] ParticleSystem _particle;
        [SerializeField] Type _type;

        bool _isUsed;
        float _elapsed;

        void Start()
        {
            StartCoroutine(RotateAsync());
            _particle.Play();
        }

        void Update()
        {
            // 画面に大量に存在する状況を防ぐため、一定時間経過で使用済みフラグを立て、削除する。
            _elapsed += Time.deltaTime;
            if (_elapsed > 10.0f) _isUsed = true; // 適当に時間を指定。

            // 時間経過もしくは取得されて、使用済みフラグが立った場合は削除される。
            if (_isUsed)
            {
                DungeonManager.RemoveActor(Coords, this);
                Destroy(gameObject);
            }
        }

        public string Scavenge(Actor user, out Item item)
        {
            if (_isUsed)
            {
                item = null;
                return "Empty";
            }

            _isUsed = true;

            if (_type == Type.Light)
            {
                item = new LightKey();
            }
            else
            {
                item = new HeavyKey();
            }

            return "Get";
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
