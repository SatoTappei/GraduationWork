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
        [SerializeField] AvatarCustomizeData[] _males;
        [SerializeField] AvatarCustomizeData[] _females;

        public AvatarCustomizeData GetCustomizedData(SpreadSheetData profile)
        {
            // とりあえず性別でのみ判定する。
            // 後々、AI側にキャラクターの見た目の選択肢を提示して、その中から選ぶような処理を追加する。
            if (profile.Sex == "男性" || profile.Sex == "男")
            {
                return GetRandomMale();
            }
            else if (profile.Sex == "女性" || profile.Sex == "女")
            {
                return GetRandomFemale();
            }

            // 男女以外の場合はランダムなキャラクターを選択。
            if (Random.value < 0.5f) return GetRandomMale();
            else return GetRandomFemale();
        }

        AvatarCustomizeData GetRandomMale() => _males[Random.Range(0, _males.Length)];
        AvatarCustomizeData GetRandomFemale() => _females[Random.Range(0, _males.Length)];
    }
}
