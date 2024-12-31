using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Barrel : DungeonEntity, IScavengeable
    {
        static readonly BilingualString[] Table =
        {
            new BilingualString("荷物", "Luggage"), // 「依頼されたアイテムの入手」達成に必要。
            new BilingualString("ガラクタ", "Junk"),
            new BilingualString("錆びた武器", "RustyWeapon"),
            new BilingualString("巻物", "Scroll"),
            new BilingualString("薬草", "Herb"),
            new BilingualString("水", "Water"),
            new BilingualString("金貨", "Coin"),
        };

        [SerializeField] float _refillInterval = 10.0f;
        [SerializeField] ParticleSystem _particle;

        WaitForSeconds _waitRefill;
        bool _isEmpty;

        void Start()
        {
            DungeonManager.AddAvoidCell(Coords);
        }

        public Item Scavenge()
        {
            PlaySE();
            _particle.Play();

            if (_isEmpty) return null;
            else _isEmpty = true;

            StartCoroutine(RefillAsync());

            // ランダムなアイテム。
            BilingualString str = Table[Random.Range(0, Table.Length)];
            return new Item(str.Japanese, str.English);
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