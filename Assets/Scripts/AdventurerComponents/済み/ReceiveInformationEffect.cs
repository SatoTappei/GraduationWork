using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ReceiveInformationEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particle;
        [SerializeField] AudioClip _se;

        AudioSource _audioSource;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play()
        {
            _particle.Play();

            _audioSource.clip = _se;
            _audioSource.Play();
        }
    }
}
