using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Container : DungeonEntity, IScavengeable
    {
        [SerializeField] ParticleSystem _particle;

        void Start()
        {
            DungeonManager.Find().AddAvoidCell(Coords);
        }

        public Item Scavenge()
        {
            PlaySE();
            _particle.Play();

            return new Item("依頼されたアイテム", "RequestedItem");
        }

        void PlaySE()
        {
            if (TryGetComponent(out AudioSource source)) source.Play();
        }
    }
}
