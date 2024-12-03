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
            DungeonManager.TryFind(out DungeonManager dungeonManager);
            dungeonManager.AddAvoidCell(Coords);
        }

        public Item Scavenge()
        {
            PlaySE();
            _particle.Play();

            return new Item("ˆË—Š‚³‚ê‚½ƒAƒCƒeƒ€", "RequestedItem");
        }

        void PlaySE()
        {
            if (TryGetComponent(out AudioSource source)) source.Play();
        }
    }
}
