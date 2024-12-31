using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Barrel : DungeonEntity, IScavengeable
    {
        static readonly BilingualString[] Table =
        {
            new BilingualString("�ו�", "Luggage"), // �u�˗����ꂽ�A�C�e���̓���v�B���ɕK�v�B
            new BilingualString("�K���N�^", "Junk"),
            new BilingualString("�K�т�����", "RustyWeapon"),
            new BilingualString("����", "Scroll"),
            new BilingualString("��", "Herb"),
            new BilingualString("��", "Water"),
            new BilingualString("����", "Coin"),
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

            // �����_���ȃA�C�e���B
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