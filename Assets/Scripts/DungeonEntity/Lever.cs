using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.ItemData;

namespace Game
{
    // レバーで制御する対象などに実装するインターフェース。
    public interface ILeverControllable
    {
        public void Play();
    }

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
            DungeonManager.AddAvoidCell(Coords);

            // このレバーに対応する仕掛けが正常に設定されているかチェック。
            if (LeverBind.GetTargetCoords(Coords).Count == 0)
            {
                Debug.LogWarning($"レバーに対応する仕掛けが無い。{Coords}");
            }
        }

        // タルやコンテナと同じく、漁ることで仕掛けが動作する。
        // 行動ログには「漁ったが何も手に入らなかった。」と記録される。
        public string Scavenge(Actor _, out Item item)
        {
            if (_isPlaying)
            {
                item = null;
                return "Empty";
            }

            StartCoroutine(PlayAsync());

            item = null;
            return "Empty";
        }

        IEnumerator PlayAsync()
        {
            _isPlaying = true;

            yield return RotateAsync(_closeRotation, _openRotation);

            // このレバーで起動するオブジェクトの座標を取得。
            IReadOnlyList<Vector2Int> targetCoords = LeverBind.GetTargetCoords(Coords);

            // このレバーで起動するオブジェクトが複数ある場合、ランダムに1つ選ぶ。
            int random = Random.Range(0, targetCoords.Count);
            foreach (Actor actor in DungeonManager.GetActors(targetCoords[random]))
            {
                if (actor.TryGetComponent(out ILeverControllable target)) target.Play();
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
