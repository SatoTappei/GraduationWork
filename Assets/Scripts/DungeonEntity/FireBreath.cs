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

        DungeonManager _dungeonManager;
        WaitForSeconds _waitDuration;
        bool _isPlaying;

        void Start()
        {
            DungeonManager.TryFind(out _dungeonManager);

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
                Cell c = _dungeonManager.GetCell(coords);
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
                _dungeonManager.SetCellTerrainEffect(coords, TerrainEffect.Flaming);
            }

            // �����o�Ă��鎞�ԁB���Ԋu�Ń_���[�W��^����B
            for (int i = 0; i <= Duration; i++)
            {
                for (int k = 0; k <= Range; k++)
                {
                    Vector2Int coords = Coords + Direction * k;
                    foreach (Actor actor in _dungeonManager.GetActorsOnCell(coords))
                    {
                        if (actor is Adventurer adventurer)
                        {
                            adventurer.Damage(
                                nameof(FireBreath), 
                                nameof(FireBreath), 
                                Damage, 
                                coords
                            );
                        }
                    }
                }

                yield return _waitDuration ??= new WaitForSeconds(Interval);
            }

            // �͈͓��̃Z���ɉ���̌��ʂ��폜�B
            for (int i = 0; i <= Range; i++)
            {
                Vector2Int coords = Coords + Direction * i;
                _dungeonManager.DeleteCellTerrainEffect(coords);
            }

            _particle.Stop();
            _isPlaying = false;
        }

        bool IsSpaceAvailable(Vector2Int direction)
        {
            for (int i = 1; i <= Range; i++)
            {
                Vector2Int coords = Coords + direction * i;
                if (_dungeonManager.GetCell(coords).IsImpassable())
                {
                    return false;
                }
            }

            return true;
        }
    }
}