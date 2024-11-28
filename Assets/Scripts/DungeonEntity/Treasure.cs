using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Treasure : DungeonEntity, IScavengeable
    {
        [SerializeField] MeshRenderer _closeChest;
        [SerializeField] MeshRenderer _openChest;
        [SerializeField] MeshRenderer _coins;
        [SerializeField] ParticleSystem _kirakiraParticle;
        [SerializeField] ParticleSystem _smokeParticle;

        DungeonManager _dungeonManager;
        AudioSource _audioSource;
        WaitForSeconds _waitOpenEffect;
        WaitForSeconds _waitScavengeInterval;
        bool _isOpen;

        public bool IsEmpty => _isOpen;

        void Awake()
        {
            _dungeonManager = DungeonManager.Find();
            _audioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            _openChest.enabled = false;
            _coins.enabled = true;
            _closeChest.enabled = true;

            _dungeonManager.AddAvoidCell(Coords);
        }

        public Item Scavenge()
        {
            if (_isOpen)
            {
                return null;
            }
            else
            {
                StartCoroutine(OpenAsync());
                return new Item("宝物", "Treasure");
            }
        }

        IEnumerator OpenAsync()
        {
            _isOpen = true;
            _dungeonManager.RemoveAvoidCell(Coords);

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
            _dungeonManager.AddAvoidCell(Coords);
        }
    }
}
