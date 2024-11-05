using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class Extensions
    {
        /// <summary>
        /// 子オブジェクトに対して再帰的にFindする。
        /// </summary>
        public static Transform FindChildRecursive(this Transform parent, string name)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name.Contains(name)) return child;

                Transform result = child.FindChildRecursive(name);
                if (result != null) return result;
            }

            return null;
        }

        /// <summary>
        /// 親を含めたTryGetComponentメソッド。
        /// </summary>
        public static bool TryGetComponentInParent<T>(this Component component, out T t)
        {
            t = component.GetComponentInParent<T>();
            return t != null;
        }
    }
}
