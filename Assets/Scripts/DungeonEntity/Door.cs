using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class Door : DungeonEntity
    {
        [SerializeField] AudioClip _openSE;
        [SerializeField] AudioClip _closeSE;

        Transform _rotateAxis;
        AudioSource _audioSource;
        WaitForSeconds _keepOpen;
        bool _isPlaying;

        void Awake()
        {
            _rotateAxis = transform.Find("RotateAxis");
            _audioSource = GetComponent<AudioSource>();
        }

        public override void Interact(Actor user)
        {
            if (_isPlaying) return;

            StartCoroutine(OpenAndCloseAsync());
        }

        IEnumerator OpenAndCloseAsync()
        {
            // �J����/������Ԃł̊p�x�B
            const float ClosedAngle = 0;
            const float OpenAngle = 80.0f;

            _isPlaying = true;

            _audioSource.clip = _openSE;
            _audioSource.Play();

            yield return AnimationAsync(ClosedAngle, OpenAngle);
            yield return KeepOpenAsync();

            _audioSource.clip = _closeSE;
            _audioSource.Play();

            yield return AnimationAsync(OpenAngle, ClosedAngle);

            _isPlaying = false;
        }

        IEnumerator AnimationAsync(float beginAngle, float endAngle)
        {
            for (float t = 0; t <= 1; t += Time.deltaTime)
            {
                float angle = Mathf.Lerp(beginAngle, endAngle, Easing(t));
                _rotateAxis.localRotation = Quaternion.Euler(new Vector3(0, angle, 0));

                yield return null;
            }
        }

        IEnumerator KeepOpenAsync()
        {
            // �`���҂��Ⴄ�Z���Ɉړ�����܂ő҂B
            do
            {
                // 1~2�b�Ԋu�Œ��ׂ�Ώ\���B
                yield return _keepOpen ??= new WaitForSeconds(1.0f);

            } while (DungeonManager.GetActors(Coords).Any(x => x is Adventurer));
        }

        static float Easing(float x)
        {
            if (x < 0.5f)
            {
                return 4 * x * x * x;
            }
            else
            {
                float f = -2 * x + 2;
                return 1 - f * f * f / 2;
            }
        }
    }
}
