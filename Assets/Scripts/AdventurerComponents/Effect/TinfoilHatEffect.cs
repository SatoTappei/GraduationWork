using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class TinfoilHatEffect : MonoBehaviour
    {
        [SerializeField] GameObject _prefab;
        [SerializeField] AudioClip _clip;
        [SerializeField] ParticleSystem _particle;

        WaitForSeconds _waitDuration;
        bool _isPlaying;

        void Awake()
        {
            _prefab.SetActive(false);
        }

        public void Play()
        {
            if (_isPlaying) return;

            StartCoroutine(PlayAsync());
        }

        IEnumerator PlayAsync()
        {
            _isPlaying = true;
            _prefab.SetActive(true);
            _particle.Play();

            TryGetComponent(out AudioSource audioSource);
            audioSource.clip = _clip;
            audioSource.Play();

            yield return _waitDuration ??= new WaitForSeconds(10.0f);
            
            _prefab.SetActive(false);
            _isPlaying = false;
            _particle.Stop();
        }
    }
}
