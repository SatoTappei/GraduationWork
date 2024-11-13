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

        bool _isPlaying;
        WaitForSeconds _keepOpen;

        public override void Interact(Actor user)
        {
            if (_isPlaying) return;

            StartCoroutine(OpenAndCloseAsync());
        }

        IEnumerator OpenAndCloseAsync()
        {
            // äJÇØÇΩ/ï¬Ç∂ÇΩèÛë‘Ç≈ÇÃäpìxÅB
            const float ClosedAngle = 0;
            const float OpenAngle = 80.0f;

            _isPlaying = true;

            PlayOpenSE();
            yield return AnimationAsync(ClosedAngle, OpenAngle);
            yield return KeepOpenAsync();
            PlayCloseSE();
            yield return AnimationAsync(OpenAngle, ClosedAngle);

            _isPlaying = false;
        }

        IEnumerator AnimationAsync(float beginAngle, float endAngle)
        {
            Transform axis = transform.Find("RotateAxis");
            for (float t = 0; t <= 1; t += Time.deltaTime)
            {
                float angle = Mathf.Lerp(beginAngle, endAngle, Easing(t));
                axis.localRotation = Quaternion.Euler(new Vector3(0, angle, 0));

                yield return null;
            }
        }

        IEnumerator KeepOpenAsync()
        {
            // ñ`åØé“Ç™à·Ç§ÉZÉãÇ…à⁄ìÆÇ∑ÇÈÇ‹Ç≈ë“Ç¬ÅB
            do
            {
                yield return _keepOpen ??= new WaitForSeconds(1.5f); // 1~2ïbä‘äuÇ≈í≤Ç◊ÇÍÇŒè\ï™ÅB

            } while (DungeonManager.Find().GetActorsOnCell(Coords).Any(x => x is Adventurer));
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

        void PlayOpenSE() => PlaySE(_openSE);
        void PlayCloseSE() => PlaySE(_closeSE);

        void PlaySE(AudioClip clip)
        {
            if (TryGetComponent(out AudioSource source))
            {
                source.clip = clip;
                source.Play();
            }
        }
    }
}
