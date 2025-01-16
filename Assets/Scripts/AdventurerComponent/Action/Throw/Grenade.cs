using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ItemThrower
{
    public class Grenade : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particle;
        [SerializeField] Renderer[] _renderers;

        public void Throw(Vector2Int coords, Vector2Int direction)
        {
            StartCoroutine(ThrowAsync(coords, direction));
        }

        IEnumerator ThrowAsync(Vector2Int coords, Vector2Int direction)
        {
            const int MaxDistance = 5;
            const float Speed = 2.0f;

            // 着弾するセルを予め決める。
            Cell targetCell = DungeonManager.GetCell(coords);
            for (int i = 0; i < MaxDistance; i++)
            {
                Cell cell = DungeonManager.GetCell(coords + direction * i);

                if (cell.IsPassable()) targetCell = cell;
                else break;
            }

            // 目標のセルまでの距離。0で割るとログが出て鬱陶しいので、同じセルの場合は距離を1に設定。
            float distance = 1.0f;
            if (coords != targetCell.Coords)
            {
                distance = Vector2Int.Distance(coords, targetCell.Coords);
            }

            // 目標のセルに向けて飛ばす。
            Vector2Int prevCoords = coords;
            Vector3 start = DungeonManager.GetCell(coords).Position;
            Vector3 goal = targetCell.Position;
            goal.y = -0.5f; // 地面の高さ。
            for (float t = 0; t <= 1.0f; t += Time.deltaTime / distance * Speed)
            {
                Vector2Int currentCoords = DungeonManager.GetCell(transform.position).Coords;
                
                // 別のセルに移動した際、そのセルに何らかのオブジェクトがあるかチェック。
                if (prevCoords != currentCoords && Check(currentCoords)) break;

                prevCoords = currentCoords;

                float x = Mathf.Lerp(start.x, goal.x, EasingForward(t));
                float y = Mathf.Lerp(start.y, goal.y, EasingFall(t));
                float z = Mathf.Lerp(start.z, goal.z, EasingForward(t));
                transform.position = new Vector3(x, y, z);

                yield return null;
            }

            // 爆発の演出を再生。
            foreach (Renderer r in _renderers)
            {
                r.enabled = false;
            }
            _particle.Play();

            // 演出を待ってから削除。
            yield return new WaitForSeconds(2.0f); // 時間は適当に指定。

            Destroy(gameObject);
        }

        static bool Check(Vector2Int coords)
        {
            IReadOnlyList<Actor> actors = DungeonManager.GetActors(coords);

            // 何もない、誰もいない場合。
            if (actors.Count == 0) return false;

            foreach (Actor actor in DungeonManager.GetActors(coords))
            {
                // 冒険者または敵がいる場合はダメージを与える。
                if (actor.TryGetComponent(out IDamageable damage))
                {
                    damage.Damage(70, coords); // ダメージ量は適当。
                }
            }

            return true;
        }

        static float EasingForward(float t)
        {
            float x = 1 - t;
            return 1 - (x * x * x);
        }

        static float EasingFall(float t)
        {
            return t * t * t * t;
        }
    }
}
