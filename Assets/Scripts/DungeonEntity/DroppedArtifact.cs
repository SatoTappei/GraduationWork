using Game.ItemData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DroppedArtifact : DungeonEntity, IScavengeable
    {
        [SerializeField] Transform _fbx;

        bool _isEmpty;

        void OnEnable()
        {
            // 1回の冒険終了したタイミングで落としたアーティファクトは消す仕様。
            GameManager.OnGameEnd += Destroy;
        }

        void OnDisable()
        {
            GameManager.OnGameEnd -= Destroy;
        }

        void Start()
        {
            StartCoroutine(RotateAsync());
        }

        public string Scavenge(Actor _, out Item item)
        {
            if (_isEmpty)
            {
                item = null;
                return "Empty";
            }

            _isEmpty = true;

            Destroy();

            item = new ItemData.Artifact();
            return "Get";
        }

        void Destroy()
        {
            DungeonManager.RemoveActor(Coords, this);
            Destroy(gameObject);
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
