using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class Extensions
    {
        /// <summary>
        /// �q�I�u�W�F�N�g�ɑ΂��čċA�I��Find����B
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
        /// �e���܂߂�TryGetComponent���\�b�h�B
        /// </summary>
        public static bool TryGetComponentInParent<T>(this Component component, out T t)
        {
            t = component.GetComponentInParent<T>();
            return t != null;
        }
    }
}
