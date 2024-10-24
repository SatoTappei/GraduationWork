using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Treasure : DungeonEntity, IScavengeable
    {
        [SerializeField] GameObject _closeChest;
        [SerializeField] GameObject _openChest;
        [SerializeField] ParticleSystem _particle;

        void Start()
        {
            Close();
            DungeonManager.Find().AddAvoidCell(Coords);
        }

        public override void Interact(Actor _)
        {
            Open();
            DungeonManager.Find().RemoveActorOnCell(Coords, this);

            _particle.Play();
            if (TryGetComponent(out AudioSource source)) source.Play();
        }

        public string Scavenge()
        {
            Interact(null);
            return string.Empty;
        }

        void Open() => SetChestState(isOpen: true);
        void Close() => SetChestState(isOpen: false);

        void SetChestState(bool isOpen)
        {
            _openChest.SetActive(isOpen);
            _closeChest.SetActive(!isOpen);
        }
    }
}
