using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.ItemData;

namespace Game
{
    public class Treasure : DungeonEntity, IScavengeable
    {
        [SerializeField] MeshRenderer _closeChest;
        [SerializeField] MeshRenderer _openChest;
        [SerializeField] MeshRenderer _coins;
        [SerializeField] ParticleSystem _kirakiraParticle;
        [SerializeField] ParticleSystem _smokeParticle;

        AudioSource _audioSource;
        WaitForSeconds _waitOpenEffect;
        WaitForSeconds _waitScavengeInterval;
        bool _isOpen;

        public bool IsEmpty => _isOpen;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            _openChest.enabled = false;
            _coins.enabled = true;
            _closeChest.enabled = true;

            DungeonManager.AddAvoidCell(Coords);
        }

        public string Scavenge(Actor user, out Item item)
        {
            // 鍵を持っているかチェック。
            if (!IsConsumeKey(user))
            {
                item = null;
                return "Lock";
            }

            if (_isOpen)
            {
                item = null;
                return "Empty";
            }
            else
            {
                StartCoroutine(OpenAsync());
                
                item = new ItemData.Treasure();
                return "Get";
            }
        }

        IEnumerator OpenAsync()
        {
            _isOpen = true;

            _openChest.enabled = true;
            _coins.enabled = true;
            _closeChest.enabled = false;
            _kirakiraParticle.gameObject.SetActive(true);
            _kirakiraParticle.Play();
            _audioSource.Play();

            // 宝箱を開けた演出が終わるまで待つ。時間は適当に指定。
            yield return _waitOpenEffect ??= new WaitForSeconds(3.0f);

            _coins.enabled = false;
            _kirakiraParticle.Stop();
            _kirakiraParticle.gameObject.SetActive(false);

            // 宝箱の中身を再度取得できるようになるまで待つ。時間は適当に指定。
            yield return _waitScavengeInterval ??= new WaitForSeconds(30.0f);

            _openChest.enabled = false;
            _closeChest.enabled = true;
            _smokeParticle.Play();

            _isOpen = false;
        }

        static bool IsConsumeKey(Actor actor)
        {
            if (actor.TryGetComponent(out ItemInventory itemInventory))
            {
                string key = string.Empty;
                foreach (IReadOnlyList<Item> item in itemInventory.Get().Values)
                {
                    if (item.Count == 0) continue;

                    // とりあえずどちらの鍵でも開くようにしておく。
                    if (item[0].ID == nameof(LightKey) || item[0].ID == nameof(HeavyKey))
                    {
                        key = item[0].ID;
                    }
                }

                // インベントリ内に鍵がある場合は削除。
                if (key != string.Empty)
                {
                    itemInventory.Remove(key);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}