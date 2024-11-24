using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatedEffect : MonoBehaviour
    {
        [SerializeField] AudioClip _defeatedSE;

        public void Play()
        {
            Animator animator = GetComponentInChildren<Animator>();
            AudioSource audioSource = GetComponent<AudioSource>();

            if (animator != null)
            {
                animator.Play("Death");
            }

            if (audioSource != null && _defeatedSE != null)
            {
                audioSource.clip = _defeatedSE;
                audioSource.Play();
            }
        }
    }
}
