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
                return new Item("��", "Treasure");
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

            // �󔠂��J�������o���I���܂ő҂B���Ԃ͓K���Ɏw��B
            yield return _waitOpenEffect ??= new WaitForSeconds(3.0f);

            _coins.enabled = false;
            _kirakiraParticle.Stop();
            _kirakiraParticle.gameObject.SetActive(false);

            // �󔠂̒��g���ēx�擾�ł���悤�ɂȂ�܂ő҂B���Ԃ͓K���Ɏw��B
            yield return _waitScavengeInterval ??= new WaitForSeconds(30.0f);

            _openChest.enabled = false;
            _closeChest.enabled = true;
            _smokeParticle.Play();

            _isOpen = false;
            _dungeonManager.AddAvoidCell(Coords);
        }
    }
}
