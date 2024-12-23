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

        public AvatarCustomizeData GetCustomizedData(AdventurerSpreadSheetData profile)
        {
            if (profile.Avatar == "女傭兵") return _avatars[0];
            if (profile.Avatar == "男傭兵") return _avatars[1];
            if (profile.Avatar == "少年") return _avatars[2];
            if (profile.Avatar == "勇者") return _avatars[3];
            if (profile.Avatar == "少女(黄)") return _avatars[4];
            if (profile.Avatar == "少女(赤)") return _avatars[5];
            if (profile.Avatar == "兵士") return _avatars[6];
            if (profile.Avatar == "女戦士") return _avatars[7];
            if (profile.Avatar == "男戦士") return _avatars[8];
            if (profile.Avatar == "騎士") return _avatars[9];
            if (profile.Avatar == "女魔法使い") return _avatars[10];
            if (profile.Avatar == "男魔法使い") return _avatars[11];

            Debug.LogWarning($"{profile.FullName}: {profile.Avatar}に対応するアバターが無い。");

            if (profile.Sex == "男性" || profile.Sex == "男")
            {
                return _avatars[2];
            }
            else if (profile.Sex == "女性" || profile.Sex == "女")
            {
                return _avatars[4];
            }
            else
            {
                return _avatars[2];
            }
        }
    }
}
