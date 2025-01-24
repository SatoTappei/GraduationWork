using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // 任意のタイミングで登録、破棄されるタイミングで削除しているだけ。
    // わざわざコンポーネントを分ける必要ないかもしれないが、他のUIと同じ手順になりわかりやすい？
    public class NameTagBinder : MonoBehaviour
    {
        Adventurer _adventurer;
        NameTag _nameTag;
        bool _isRegisterd;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _nameTag = NameTag.Find();
        }

        public void Register()
        {
            if (_isRegisterd)
            {
                Debug.LogWarning("既に登録済み。");
            }
            else
            {
                _isRegisterd = true;
                _nameTag.Register(_adventurer);
            }
        }

        void OnDestroy()
        {
            if (_nameTag != null)
            {
                _nameTag.Delete(_adventurer);
            }
        }
    }
}
