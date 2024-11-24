using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class TalkEffect : MonoBehaviour
    {
        ParticleSystem _particle;

        void Awake()
        {
            _particle = transform.FindChildRecursive("Particle_Talk").GetComponent<ParticleSystem>();
        }

        public void Play()
        {
            _particle.Play();
        }
    }
}
