using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EnemySpawnEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particle;

        public void Play()
        {
            if (_particle != null)
            {
                _particle.Play();
            }
        }
    }
}
