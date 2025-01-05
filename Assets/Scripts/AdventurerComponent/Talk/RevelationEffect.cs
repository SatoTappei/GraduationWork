using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RevelationEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particle;
        [SerializeField] AudioClip _revelationSE;

        AudioSource _audioSource;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play()
        {
            _particle.Play();

            _audioSource.clip = _revelationSE;
            _audioSource.Play();
        }
    }
}
