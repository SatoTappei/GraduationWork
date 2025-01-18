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

            // このコンテナから取得できるアイテムのデータが正常に設定されているかチェック。
            if (ContainerContents.GetItems(Coords).Count == 0)
            {
                Debug.LogWarning($"コンテナから取得できるアイテムが無い。{Coords}");
            }
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
                // 一定時間後、空っぽフラグを立てておく。
                StartCoroutine(RefillAsync());

                // ランダムなアイテム。
                IReadOnlyList<string> items = ContainerContents.GetItems(Coords);
                string item = items[Random.Range(0, items.Count)];
                if (item == "荷物") return new Luggage(); // 「依頼されたアイテムの入手」達成に必要。
                if (item == "ガラクタ") return new Junk();
                if (item == "錆びた剣") return new RustySword();
                if (item == "クラッカー") return new Cracker();
                if (item == "壊れた罠") return new BrokenTrap();
                if (item == "切れた電球") return new LightBlub();
                if (item == "ヘルメット") return new Helmet();
                if (item == "手榴弾") return new Grenade();

                Debug.LogWarning($"対応した返すアイテムが無い。{item}");
                return new Junk();
            }
        }

        IEnumerator RefillAsync()
        {
            _isEmpty = true;

            float interval = ContainerContents.GetInterval(Coords);
            yield return _waitRefill ??= new WaitForSeconds(interval);
            
            _isEmpty = false;
        }
    }
}
