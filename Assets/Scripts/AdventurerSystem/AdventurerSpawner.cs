using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class AdventurerSpawner : MonoBehaviour
    {
        [SerializeField] Vector2Int _spawnCoords;

        List<Adventurer> _spawned;

        public IReadOnlyList<Adventurer> Spawned => _spawned;

        void Awake()
        {
            _spawned = new List<Adventurer>();
        }

        public static bool TryFind(out AdventurerSpawner result)
        {
            result = GameObject.FindGameObjectWithTag("AdventurerSpawner").GetComponent<AdventurerSpawner>();
            return result != null;
        }

        public async UniTask<int> SpawnAsync(int count, CancellationToken token)
        {
            IReadOnlyList<AdventurerData>  profiles = await AdventurerLoader.GetDataAsync(token);

            // 読み込んだプロフィールの数が、引数で指定した数より少ない場合を考慮。
            int spawnedCount = Mathf.Min(profiles.Count, count);
            for (int i = 0; i < spawnedCount;)
            {
                if (IsCellEmpty()) Spawn(profiles[i++]);

                await UniTask.WaitForSeconds(1.0f, cancellationToken: token); // 適当な秒数。
            }

            return spawnedCount;
        }

        bool IsCellEmpty()
        {
            foreach (Actor actor in DungeonManager.GetActorsOnCell(_spawnCoords))
            {
                if (actor is Adventurer _) return false;
            }

            return true;
        }

        void Spawn(AdventurerData profile)
        {
            TryGetComponent(out AvatarCustomizer avatarCustomizer);
            AvatarCustomizeData avatarData = avatarCustomizer.GetCustomizedData(profile);

            // 冒険者側に自身のプロフィールとコメント、アバターの情報を渡して初期化する。
            AdventurerSheet adventurerSheet = new AdventurerSheet(profile, avatarData);
            Adventurer adventurer = Instantiate(avatarData.Prefab);
            adventurer.Initialize(adventurerSheet);

            _spawned.Add(adventurer);
        }
    }
}