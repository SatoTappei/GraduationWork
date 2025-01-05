using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FireBreath : DungeonEntity
    {
        // パーティクルの見た目も合わせて調整が必要。
        const int Range = 3;

        [SerializeField] Transform _parent;
        [SerializeField] ParticleSystem _particle;

        WaitForSeconds _waitDuration;
        bool _isPlaying;

        void Start()
        {
            List<Vector2Int> candidate = new List<Vector2Int>();
            if (IsSpaceAvailable(Vector2Int.up)) candidate.Add(Vector2Int.up);
            if (IsSpaceAvailable(Vector2Int.down)) candidate.Add(Vector2Int.down);
            if (IsSpaceAvailable(Vector2Int.right)) candidate.Add(Vector2Int.right);
            if (IsSpaceAvailable(Vector2Int.left)) candidate.Add(Vector2Int.left);

            // ランダムな方向を向けて配置。
            Vector2Int direction = candidate[Random.Range(0, candidate.Count)];
            Place(Coords, direction);
        }

        void OnDrawGizmosSelected()
        {
            if (!_isPlaying) return;

            // 効果範囲、炎上中のセルを描画。
            for (int i = 0; i <= Range; i++)
            {
                Vector2Int coords = Coords + Direction * i;
                Cell c = DungeonManager.GetCell(coords);
                if (c.TerrainEffect == TerrainEffect.Flaming)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(c.Position, Vector3.one);
                }
            }
        }

        public void Play()
        {
            if(_isPlaying) return;
            
            StartCoroutine(PlayAsync());
        }

        bool IsSpaceAvailable(Vector2Int direction)
        {
            for (int i = 1; i <= Range; i++)
            {
                Vector2Int coords = Coords + direction * i;
                if (DungeonManager.GetCell(coords).IsImpassable())
                {
                    return false;
                }
            }

            return true;
        }

        IEnumerator PlayAsync()
        {
            const int Duration = 30;
            const int Damage = 5;
            const float Interval = 1.0f;

            _isPlaying = true;
            _particle.Play();

            // 範囲内のセルに炎上の効果を付与。
            for (int i = 0; i <= Range; i++)
            {
                Vector2Int coords = Coords + Direction * i;
                DungeonManager.SetTerrainEffect(coords, TerrainEffect.Flaming);
            }

            // 炎が出ている時間。一定間隔でダメージを与える。
            for (int i = 0; i <= Duration; i++)
            {
                for (int k = 0; k <= Range; k++)
                {
                    Vector2Int coords = Coords + Direction * k;
                    foreach (Actor actor in DungeonManager.GetActors(coords))
                    {
                        if (actor != null && actor.TryGetComponent(out IDamageable damage))
                        {
                            damage.Damage(Damage, coords);
                        }
                    }
                }

                yield return _waitDuration ??= new WaitForSeconds(Interval);
            }

            // 範囲内のセルに炎上の効果を削除。
            for (int i = 0; i <= Range; i++)
            {
                Vector2Int coords = Coords + Direction * i;
                DungeonManager.DeleteTerrainEffect(coords);
            }

            _particle.Stop();
            _isPlaying = false;
        }
    }
}