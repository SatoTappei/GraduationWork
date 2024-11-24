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
            AudioSource audioSource = GetComponent<AudioSource>();

            if (audioSource != null && _entrySE != null)
            {
                audioSource.clip = _entrySE;
                audioSource.Play();
            }
        }
    }
}