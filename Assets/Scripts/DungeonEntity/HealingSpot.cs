using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HealingSpot : DungeonEntity
    {
        [SerializeField] Transform _fbx;

        void Start()
        {
            StartCoroutine(RotateAsync());
        }

        IEnumerator RotateAsync()
        {
            Vector3 basePosition = _fbx.localPosition;
            while (true)
            {
                // アイテムが回転しつつ浮遊している感じの動き。値は適当。
                _fbx.Rotate(Vector3.up * Time.deltaTime * 20.0f);
                _fbx.localPosition = basePosition + Vector3.up * Mathf.Sin(Time.time) * 0.2f;

                yield return null;
            }
        }
    }
}
