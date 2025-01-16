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

            // ���e����Z����\�ߌ��߂�B
            Cell targetCell = DungeonManager.GetCell(coords);
            for (int i = 0; i < MaxDistance; i++)
            {
                Cell cell = DungeonManager.GetCell(coords + direction * i);

                if (cell.IsPassable()) targetCell = cell;
                else break;
            }

            // �ڕW�̃Z���܂ł̋����B0�Ŋ���ƃ��O���o�ğT�������̂ŁA�����Z���̏ꍇ�͋�����1�ɐݒ�B
            float distance = 1.0f;
            if (coords != targetCell.Coords)
            {
                distance = Vector2Int.Distance(coords, targetCell.Coords);
            }

            // �ڕW�̃Z���Ɍ����Ĕ�΂��B
            Vector2Int prevCoords = coords;
            Vector3 start = DungeonManager.GetCell(coords).Position;
            Vector3 goal = targetCell.Position;
            goal.y = -0.5f; // �n�ʂ̍����B
            for (float t = 0; t <= 1.0f; t += Time.deltaTime / distance * Speed)
            {
                Vector2Int currentCoords = DungeonManager.GetCell(transform.position).Coords;
                
                // �ʂ̃Z���Ɉړ������ہA���̃Z���ɉ��炩�̃I�u�W�F�N�g�����邩�`�F�b�N�B
                if (prevCoords != currentCoords && Check(currentCoords)) break;

                prevCoords = currentCoords;

                float x = Mathf.Lerp(start.x, goal.x, EasingForward(t));
                float y = Mathf.Lerp(start.y, goal.y, EasingFall(t));
                float z = Mathf.Lerp(start.z, goal.z, EasingForward(t));
                transform.position = new Vector3(x, y, z);

                yield return null;
            }

            // �����̉��o���Đ��B
            foreach (Renderer r in _renderers)
            {
                r.enabled = false;
            }
            _particle.Play();

            // ���o��҂��Ă���폜�B
            yield return new WaitForSeconds(2.0f); // ���Ԃ͓K���Ɏw��B

            Destroy(gameObject);
        }

        static bool Check(Vector2Int coords)
        {
            IReadOnlyList<Actor> actors = DungeonManager.GetActors(coords);

            // �����Ȃ��A�N�����Ȃ��ꍇ�B
            if (actors.Count == 0) return false;

            foreach (Actor actor in DungeonManager.GetActors(coords))
            {
                // �`���҂܂��͓G������ꍇ�̓_���[�W��^����B
                if (actor.TryGetComponent(out IDamageable damage))
                {
                    damage.Damage(70, coords); // �_���[�W�ʂ͓K���B
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
