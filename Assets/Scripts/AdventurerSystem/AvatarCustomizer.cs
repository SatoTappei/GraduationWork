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

    // 現状、バリエーションが少ないのでインスペクターに割り当てている。
    // 必要に応じてAssetBundleなどで指定したものを動的にロードする仕組みにする。
    public class AvatarCustomizer : MonoBehaviour
    {
        [SerializeField] AvatarCustomizeData[] _avatars;

        public AvatarCustomizeData GetCustomizedData(AdventurerSpreadSheetData profile)
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

        AvatarCustomizeData GetRandomMale()
        {

            return _avatars[2]; // ChatGPTにアバターの見た目まで決めさせるまで仮。
        }

        AvatarCustomizeData GetRandomFemale()
        {
            return _avatars[4]; // ChatGPTにアバターの見た目まで決めさせるまで仮。
        }
    }
}
