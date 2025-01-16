using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.ItemData;

namespace Game
{
    public class Container : DungeonEntity, IScavengeable
    {
        [SerializeField] float _refillInterval = 10.0f;
        [SerializeField] ParticleSystem _particle;

        AudioSource _audioSource;
        WaitForSeconds _waitRefill;
        bool _isEmpty;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            DungeonManager.AddAvoidCell(Coords);
        }

        public Item Scavenge()
        {
            _audioSource.Play();
            _particle.Play();

            if (_isEmpty)
            {
                return null;
            }
            else
            {
                // ��莞�Ԍ�A����ۃt���O�𗧂ĂĂ����B
                StartCoroutine(RefillAsync());

                // �����_���ȃA�C�e���B
                int r = Random.Range(0, 8);
                if (r == 0) return new Luggage(); // �u�˗����ꂽ�A�C�e���̓���v�B���ɕK�v�B
                if (r == 1) return new Junk();
                if (r == 2) return new RustySword();
                if (r == 3) return new Scroll();
                if (r == 4) return new Herb();
                if (r == 5) return new Water();
                if (r == 6) return new Coin();
                if (r == 7) return new Grenade();

                Debug.LogWarning("�ԍ��ɑΉ������Ԃ��A�C�e���������B");
                return new Junk();
            }
        }

        IEnumerator RefillAsync()
        {
            _isEmpty = true;
            yield return _waitRefill ??= new WaitForSeconds(_refillInterval);
            _isEmpty = false;
        }
    }
}
