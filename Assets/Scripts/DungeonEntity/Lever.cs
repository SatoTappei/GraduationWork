using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.ItemData;

namespace Game
{
    // ���o�[�Ő��䂷��ΏۂȂǂɎ�������C���^�[�t�F�[�X�B
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

            // ���̃��o�[�ɑΉ�����d�|��������ɐݒ肳��Ă��邩�`�F�b�N�B
            if (LeverBind.GetTargetCoords(Coords).Count == 0)
            {
                Debug.LogWarning($"���o�[�ɑΉ�����d�|���������B{Coords}");
            }
        }

        // �^����R���e�i�Ɠ������A���邱�ƂŎd�|�������삷��B
        // �s�����O�ɂ́u��������������ɓ���Ȃ������B�v�ƋL�^�����B
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

            // ���̃��o�[�ŋN������I�u�W�F�N�g�̍��W���擾�B
            IReadOnlyList<Vector2Int> targetCoords = LeverBind.GetTargetCoords(Coords);

            // ���̃��o�[�ŋN������I�u�W�F�N�g����������ꍇ�A�����_����1�I�ԁB
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
