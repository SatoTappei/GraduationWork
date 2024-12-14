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

        // �^����R���e�i�Ɠ������A���邱�ƂŎd�|�������삷��B
        // �s�����O�ɂ́u��������������ɓ���Ȃ������B�v�ƋL�^�����B
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

            // ���̃��o�[�ŋN������I�u�W�F�N�g�̍��W���擾�B
            DungeonManager.TryFind(out DungeonManager dungeonManager);
            IReadOnlyList<Vector2Int> targetCoords = LeverBinding.GetTargetCoords(Coords);

            // ���̃��o�[�ŋN������I�u�W�F�N�g����������ꍇ�A�����_����1�I�ԁB
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
