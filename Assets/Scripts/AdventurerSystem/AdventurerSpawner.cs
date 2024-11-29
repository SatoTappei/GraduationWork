using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class AdventurerSpawner : MonoBehaviour
    {
        [SerializeField] Vector2Int _spawnCoords;

        AdventurerSpreadSheetLoader _adventurerLoader;
        AvatarCustomizer _avatarCustomizer;

        Adventurer[] _spawned;
        WaitForSeconds _waitInterval;

        public IReadOnlyList<Adventurer> Spawned => _spawned;

        void Awake()
        {
            _adventurerLoader = GetComponent<AdventurerSpreadSheetLoader>();
            _avatarCustomizer = GetComponent<AvatarCustomizer>();

            // 冒険者の最大数、UIのデザインも変更する必要があり面倒なので、とりあえず4で固定。
            _spawned = new Adventurer[4];
        }

        void Start()
        {
            StartCoroutine(UpdateAsync());
        }

        public static AdventurerSpawner Find()
        {
            return GameObject.FindGameObjectWithTag("AdventurerSpawner").GetComponent<AdventurerSpawner>();
        }

        public static bool TryFind(out AdventurerSpawner result)
        {
            result = Find();
            return result != null;
        }

        // 一定間隔で冒険者を生成する。
        IEnumerator UpdateAsync()
        {
            while (true)
            {
                Spawn();
                yield return _waitInterval ??= new WaitForSeconds(1.0f); // 適当な秒数。
            }
        }

        // シーン上に存在する冒険者が最大数未満の場合は生成する。
        void Spawn()
        {
            for (int i = 0; i < _spawned.Length; i++)
            {
                if (_spawned[i] != null) continue;
                if (IsAdventurerExist(_spawnCoords)) continue;

                _spawned[i] = CreateRandomAdventurer();
                break;
            }
        }

        // 既に冒険者がいるかチェック。
        static bool IsAdventurerExist(Vector2Int coords)
        {
            if (!DungeonManager.TryFind(out DungeonManager dungeonManager)) return false;

            foreach (Actor actor in dungeonManager.GetActorsOnCell(coords))
            {
                if (actor is Adventurer _) return true;
            }

            return false;
        }

        // スプレッドシートからランダムなデータを選択し、冒険者を生成。
        Adventurer CreateRandomAdventurer()
        {
            if (_adventurerLoader.IsLoading) return null;

            // 生成済みの冒険者の名前。
            IEnumerable<string> spawnedNames = 
                _spawned
                .Where(a => a != null)
                .Select(a => a._AdventurerSheet.FullName);

            // 生成していない冒険者のデータのみを抽出。
            IReadOnlyList<AdventurerSpreadSheetData> candidate = 
                _adventurerLoader.Profiles
                .Where(p => !spawnedNames.Contains(p.FullName))
                .ToArray();

            AdventurerSpreadSheetData profile = candidate[Random.Range(0, candidate.Count)];
            AvatarCustomizeData avatarData = _avatarCustomizer.GetCustomizedData(profile);
            
            // 冒険者側に自身のプロフィールとコメント、アバターの情報を渡して初期化する。
            AdventurerSheet adventurerSheet = new AdventurerSheet(profile, avatarData);
            Adventurer adventurer = Instantiate(avatarData.Prefab);
            adventurer.Initialize(adventurerSheet);

            return adventurer;
        }
    }
}