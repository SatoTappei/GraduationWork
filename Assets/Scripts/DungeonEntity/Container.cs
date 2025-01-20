using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.ItemData;

namespace Game
{
    public class Container : DungeonEntity, IScavengeable
    {
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

            // ���̃R���e�i����擾�ł���A�C�e���̃f�[�^������ɐݒ肳��Ă��邩�`�F�b�N�B
            if (ContainerBind.GetItems(Coords).Count == 0)
            {
                Debug.LogWarning($"�R���e�i����擾�ł���A�C�e���������B{Coords}");
            }
        }

        public string Scavenge(Actor _, out Item item)
        {
            _audioSource.Play();
            _particle.Play();

            if (_isEmpty)
            {
                item = null;
                return "Empty";
            }
            else
            {
                // ��莞�Ԍ�A����ۃt���O�𗧂ĂĂ����B
                StartCoroutine(RefillAsync());

                item = GetItem(Coords);
                return "Get";
            }
        }

        IEnumerator RefillAsync()
        {
            _isEmpty = true;

            float interval = ContainerBind.GetInterval(Coords);
            yield return _waitRefill ??= new WaitForSeconds(interval);
            
            _isEmpty = false;
        }

        static Item GetItem(Vector2Int coords)
        {
            IReadOnlyList<string> items = ContainerBind.GetItems(coords);
            string item = items[Random.Range(0, items.Count)];
            if (item == "�ו�") return new Luggage(); // �u�˗����ꂽ�A�C�e���̓���v�B���ɕK�v�B
            if (item == "�K���N�^") return new Junk();
            if (item == "�K�т���") return new RustySword();
            if (item == "�N���b�J�[") return new Cracker();
            if (item == "��ꂽ�") return new BrokenTrap();
            if (item == "�؂ꂽ�d��") return new LightBlub();
            if (item == "�w�����b�g") return new Helmet();
            if (item == "��֒e") return new Grenade();

            Debug.LogWarning($"�Ή������Ԃ��A�C�e���������B{item}");
            return new Junk();
        }
    }
}
