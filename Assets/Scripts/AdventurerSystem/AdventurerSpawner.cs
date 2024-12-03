using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class AdventurerSpawner : MonoBehaviour
    {
        [SerializeField] Vector2Int _spawnCoords;

        AvatarCustomizer _avatarCustomizer;
        IReadOnlyList<AdventurerSpreadSheetData> _profiles;
        Adventurer[] _spawned;

        public IReadOnlyList<Adventurer> Spawned => _spawned;

        void Awake()
        {
            _avatarCustomizer = GetComponent<AvatarCustomizer>();

            // 冒険者の最大数、UIのデザインも変更する必要があり面倒なので、とりあえず4で固定。
            _spawned = new Adventurer[4];
        }

        public static bool TryFind(out AdventurerSpawner result)
        {
            result = GameObject.FindGameObjectWithTag("AdventurerSpawner").GetComponent<AdventurerSpawner>();
            return result != null;
        }

        // 人数を指定し、一定間隔で冒険者を生成する。実際に生成した数を返す。
        public async UniTask<int> SpawnAsync(int count, CancellationToken token)
        {
            // 冒険者のプロフィールを読み込む。
            _profiles = await AdventurerSpreadSheetLoader.GetDataAsync(token);

            // 読み込んだプロフィールの数が、引数で指定した数より少ない場合がある。
            int spawnedCount = Mathf.Min(_profiles.Count, count);

            for (int i = 0; i < spawnedCount;)
            {
                if (TrySpawn()) i++;
                await UniTask.WaitForSeconds(1.0f, cancellationToken: token); // 適当な秒数。
            }

            return spawnedCount;
        }

        // 生成する座標に冒険者がいないかつ、シーン上に存在する冒険者が最大数未満の場合は生成する。
        bool TrySpawn()
        {
            if (IsAdventurerExist(_spawnCoords)) return false;

            for (int i = 0; i < _spawned.Length; i++)
            {
                if (_spawned[i] != null) continue;

                _spawned[i] = CreateRandomAdventurer();

                if (_spawned[i] != null) return true;
            }

            return false;
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
            // 生成済みの冒険者の名前。
            IEnumerable<string> spawnedNames = 
                _spawned
                .Where(a => a != null)
                .Select(a => a.AdventurerSheet.FullName);

            // 生成していない冒険者のデータのみを抽出。
            IReadOnlyList<AdventurerSpreadSheetData> candidate = 
                _profiles
                .Where(p => !spawnedNames.Contains(p.FullName))
                .ToArray();

            if (candidate.Count == 0) return null;

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