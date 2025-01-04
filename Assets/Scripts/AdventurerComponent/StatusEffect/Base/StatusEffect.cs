using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class StatusEffect : MonoBehaviour
    {
        // 効果を反映する。
        public abstract void Apply();

        // UIに表示する状態異常の説明文。
        public abstract string Description { get; }

        // 状態異常が現在有効かどうかを判定する。
        public abstract bool IsValid { get; }
    }
}
