using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FireBreath : DungeonEntity
    {
        // �p�[�e�B�N���̌����ڂ����킹�Ē������K�v�B
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

            // �����_���ȕ����������Ĕz�u�B
            Vector2Int direction = candidate[Random.Range(0, candidate.Count)];
            Place(Coords, direction);
        }

        void OnDrawGizmosSelected()
        {
            if (!_isPlaying) return;

            // ���ʔ͈́A���㒆�̃Z����`��B
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

            // �͈͓��̃Z���ɉ���̌��ʂ�t�^�B
            for (int i = 0; i <= Range; i++)
            {
                Vector2Int coords = Coords + Direction * i;
                DungeonManager.SetTerrainEffect(coords, TerrainEffect.Flaming);
            }

            // �����o�Ă��鎞�ԁB���Ԋu�Ń_���[�W��^����B
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

            // �͈͓��̃Z���ɉ���̌��ʂ��폜�B
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