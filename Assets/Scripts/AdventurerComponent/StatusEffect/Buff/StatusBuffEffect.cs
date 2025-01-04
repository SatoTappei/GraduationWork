using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatusBuffEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particle;
        [SerializeField] AudioClip _statusBuffSE;

        AudioSource _audioSource;
        
        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play()
        {
            _particle.Play();

            _audioSource.clip = _statusBuffSE;
            _audioSource.Play();
        }

        public void Stop()
        {
            _particle.Stop();
        }
    }
}
