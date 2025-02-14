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
        [System.Serializable]
        class AvatarData
        {
            [SerializeField] Sprite _icon;
            [SerializeField] Adventurer _prefab;

            public Sprite Icon => _icon;
            public Adventurer Prefab => _prefab;
        }

        [SerializeField] AvatarData[] _avatars;
        [SerializeField] Vector2Int _spawnCoords;

        List<Adventurer> _spawned;

        public IReadOnlyList<Adventurer> Spawned => _spawned;

        void Awake()
        {
            _spawned = new List<Adventurer>();
        }

        public static AdventurerSpawner Find()
        {
            return GameObject.FindGameObjectWithTag("GameManager").GetComponent<AdventurerSpawner>();
        }

        public async UniTask<int> SpawnAsync(ArtifactInfo[] artifacts, CancellationToken token)
        {
            // 生成する冒険者の最大数。
            const int Max = 4;

            _spawned.Clear();

            // 読み込んだプロフィールの数が、引数で指定した数より少ない場合を考慮。
            UserData[] profiles = VantanConnect.GetMainGameUsers();
            int spawnCount = Mathf.Min(profiles.Length, Max);
            IEnumerable<int> artifactHolders = artifacts.Select(a => a.OwnerId);
            for (int i = 0; i < spawnCount;)
            {
                if (IsCellEmpty())
                {
                    // 冒険者側に自身のプロフィールとアバターの情報を渡して初期化する。
                    AdventurerData profile = APIUtility.Marshal<AdventurerData, UserData>(profiles[i]);
                    AvatarData avatarData = SelectAvatar(profile);
                    AdventurerSheet adventurerSheet = new AdventurerSheet(
                        profile, 
                        avatarData.Icon, 
                        displayID: i,
                        artifactHolders.Contains(profile.UserId)
                    );
                    Adventurer adventurer = Instantiate(avatarData.Prefab);
                    adventurer.Initialize(adventurerSheet);

                    _spawned.Add(adventurer);

                    i++;
                }

                await UniTask.WaitForSeconds(1.0f, cancellationToken: token); // 適当な秒数。
            }

            return spawnCount;
        }

        bool IsCellEmpty()
        {
            foreach (Actor actor in DungeonManager.GetActors(_spawnCoords))
            {
                if (actor is Adventurer _) return false;
            }

            return true;
        }

        AvatarData SelectAvatar(AdventurerData profile)
        {
            // アバターは全12種類。
            if (0 <= profile.AvatarType && profile.AvatarType < 12)
            {
                return _avatars[profile.AvatarType];
            }
            else
            {
                Debug.LogWarning($"{profile.Name}: {profile.AvatarType} に対応するアバターが無い。");
                return _avatars[Random.Range(0, 12)];
            }
        }
    }
}