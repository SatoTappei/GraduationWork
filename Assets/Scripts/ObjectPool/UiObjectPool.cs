using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UiObjectPool<T> : ObjectPool<T> where T : Component
    {
        protected override Transform CreateParent()
        {
            GameObject g = new GameObject();
            Canvas canvas = g.AddComponent<Canvas>();
            g.AddComponent<CanvasScaler>();
            g.AddComponent<GraphicRaycaster>();

            // 以下2つのパラメータはインスペクターで割り当てるように変更しても良い。
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 99;

            return g.transform;
        }
    }
}
