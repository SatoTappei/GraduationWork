using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EntryEffect : MonoBehaviour
    {
        [SerializeField] AudioClip _entrySE;

        public void Play()
        {
            Animator animator = GetComponentInChildren<Animator>();
            AudioSource audioSource = GetComponent<AudioSource>();

            if (animator != null)
            {
                animator.Play("Entry");
            }

            if (audioSource != null && _entrySE != null)
            {
                audioSource.clip = _entrySE;
                audioSource.Play();
            }
        }
    }
}