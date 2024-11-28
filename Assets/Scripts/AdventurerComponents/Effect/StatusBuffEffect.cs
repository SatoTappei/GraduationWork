using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatusBuffEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particle;
        WaitForSeconds _waitDuration;

        bool _isPlaying;

        public void Play(float duration)
        {
            if (_isPlaying) return;

            StartCoroutine(PlayAsync(duration));
        }

        IEnumerator PlayAsync(float duration)
        {
            _isPlaying = true;
            _particle.Play();

            yield return _waitDuration ??= new WaitForSeconds(duration);

            _particle.Stop();
            _isPlaying = false;
        }
    }
}
