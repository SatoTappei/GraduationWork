using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class AvatarCustomizeData
    {
        [SerializeField] Sprite _icon;
        [SerializeField] Adventurer _prefab;

        public Sprite Icon => _icon;
        public Adventurer Prefab => _prefab;
    }

    public class AvatarCustomizer : MonoBehaviour
    {
        [SerializeField] AvatarCustomizeData[] _avatars;

        public AvatarCustomizeData GetCustomizedData(AdventurerData profile)
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
