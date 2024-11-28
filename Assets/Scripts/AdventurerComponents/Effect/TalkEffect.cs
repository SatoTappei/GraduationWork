using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class TalkEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particle;

        public void Play()
        {
            _particle.Play();
        }
    }
}
