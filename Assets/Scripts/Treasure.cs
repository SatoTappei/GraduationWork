using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Treasure : DungeonEntity
    {
        [SerializeField] GameObject _closeChest;
        [SerializeField] GameObject _openChest;
        [SerializeField] ParticleSystem _particle;

        void Awake()
        {
            Close();
        }

        public override void Interact(Actor user)
        {
            RemoveOnCell();
            Open();
            PlaySE();
            _particle.Play();
        }

        void RemoveOnCell()
        {
            DungeonManager dm = DungeonManager.Find();
            dm.RemoveActorOnCell(Coords, this);
        }

        void Open() => SetChestState(isOpen: true);
        void Close() => SetChestState(isOpen: false);

        void SetChestState(bool isOpen)
        {
            _openChest.SetActive(isOpen);
            _closeChest.SetActive(!isOpen);
        }

        void PlaySE()
        {
            if (TryGetComponent(out AudioSource source)) source.Play();
        }
    }
}
