using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ReceiveInformationEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particle;
        [SerializeField] AudioClip _receiveInformationSE;

        public void Play()
        {
            _particle.Play();

            if (TryGetComponent(out AudioSource audioSource))
            {
                audioSource.clip = _receiveInformationSE;
                audioSource.Play();
            }
        }
    }
}
