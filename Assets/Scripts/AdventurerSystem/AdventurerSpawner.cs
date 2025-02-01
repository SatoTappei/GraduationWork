using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using VTNConnect;

namespace Game
{
    public class AdventurerSpawner : MonoBehaviour
    {
        [SerializeField] Vector2Int _spawnCoords;

        AvatarCustomizer _avatarCustomizer;
        List<Adventurer> _spawned;

        public IReadOnlyList<Adventurer> Spawned => _spawned;

        void Awake()
        {
            _avatarCustomizer = GetComponent<AvatarCustomizer>();
            _spawned = new List<Adventurer>();
        }

        public static AdventurerSpawner Find()
        {
            return GameObject.FindGameObjectWithTag("AdventurerSpawner").GetComponent<AdventurerSpawner>();
        }

        public async UniTask<int> SpawnAsync(int count, CancellationToken token)
        {
            IReadOnlyList<UserData> profiles = VantanConnect.GetMainGameUsers().ToList();

            _spawned.Clear();

            // 読み込んだプロフィールの数が、引数で指定した数より少ない場合を考慮。
            int spawnedCount = Mathf.Min(profiles.Count, count);
            for (int i = 0; i < spawnedCount;)
            {
                if (IsCellEmpty())
                {
                    // 冒険者側に自身のプロフィールとコメント、アバターの情報を渡して初期化する。
                    //AdventurerData data = APIUtility.Marshal<AdventurerData, UserData>(profiles[i]); //0.4.1で対応
                    AdventurerData data = new AdventurerData(); //この行は消してね
                    AvatarData avatarData = _avatarCustomizer.GetData(data);
                    AdventurerSheet adventurerSheet = new AdventurerSheet(i, data, avatarData);
                    Adventurer adventurer = Instantiate(avatarData.Prefab);
                    adventurer.Initialize(adventurerSheet);

                    _spawned.Add(adventurer);

                    i++;
                }

                await UniTask.WaitForSeconds(1.0f, cancellationToken: token); // 適当な秒数。
            }

            return spawnedCount;
        }

        bool IsCellEmpty()
        {
            foreach (Actor actor in DungeonManager.GetActors(_spawnCoords))
            {
                if (actor is Adventurer _) return false;
            }

            return true;
        }
    }
}