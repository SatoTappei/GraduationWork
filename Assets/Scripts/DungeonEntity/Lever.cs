using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Lever : DungeonEntity, IScavengeable
    {
        [SerializeField] Transform _lever;
        [SerializeField] Vector3 _openRotation;
        [SerializeField] Vector3 _closeRotation;

        bool _isPlaying;

        void Awake()
        {
            _lever.localRotation = Quaternion.Euler(_openRotation);
        }

        void Start()
        {
            DungeonManager.TryFind(out DungeonManager dungeonManager);
            dungeonManager.AddAvoidCell(Coords);
        }

        // タルやコンテナと同じく、漁ることで仕掛けが動作する。
        // 行動ログには「漁ったが何も手に入らなかった。」と記録される。
        public Item Scavenge()
        {
            if (_isPlaying) return null;

            StartCoroutine(PlayAsync());

            return null;
        }

        IEnumerator PlayAsync()
        {
            _isPlaying = true;

            yield return RotateAsync(_closeRotation, _openRotation);

            // このレバーで起動するオブジェクトの座標を取得。
            DungeonManager.TryFind(out DungeonManager dungeonManager);
            IReadOnlyList<Vector2Int> targetCoords = LeverBinding.GetTargetCoords(Coords);

            // このレバーで起動するオブジェクトが複数ある場合、ランダムに1つ選ぶ。
            int random = Random.Range(0, targetCoords.Count);
            foreach (Actor actor in dungeonManager.GetActorsOnCell(targetCoords[random]))
            {
                if (actor is FireBreath target) target.Play();
            }

            yield return RotateAsync(_openRotation, _closeRotation);

            _isPlaying = false;
        }

        IEnumerator RotateAsync(Vector3 from, Vector3 to)
        {
            const float Speed = 1.0f;

            Quaternion fromRot = Quaternion.Euler(from);
            Quaternion toRot = Quaternion.Euler(to);
            for (float t = 0; t <= 1.0f; t += Time.deltaTime * Speed)
            {
                _lever.localRotation = Quaternion.Lerp(fromRot, toRot, Easing(t));
                yield return null;
            }

            _lever.localRotation = toRot;
        }

        static float Easing(float t)
        {
            return t * t * t * t * t;
        }
    }
}
