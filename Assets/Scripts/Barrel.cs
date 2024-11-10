using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Barrel : DungeonEntity, IScavengeable
    {
        [SerializeField] float _refillInterval = 10.0f;
        [SerializeField] ParticleSystem _particle;

        WaitForSeconds _waitRefill;
        bool _isEmpty;

        void Start()
        {
            DungeonManager.Find().AddAvoidCell(Coords);
        }

        public Item Scavenge()
        {
            PlaySE();
            _particle.Play();

            if (_isEmpty) return null;
            else _isEmpty = true;

            StartCoroutine(RefillAsync());

            return new Item("ƒKƒ‰ƒNƒ^", "Junk");
        }

        IEnumerator RefillAsync()
        {
            yield return _waitRefill ??= new WaitForSeconds(_refillInterval);
            _isEmpty = false;
        }

        void PlaySE()
        {
            if (TryGetComponent(out AudioSource source)) source.Play();
        }
    }
}