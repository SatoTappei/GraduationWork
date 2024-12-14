using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class LevitationEvent : MonoBehaviour
    {
        [SerializeField] LevitationEffect _prefab;

        LevitationEffect _effect;

        void Awake()
        {
            _effect = Instantiate(_prefab);
            _effect.gameObject.SetActive(false);
        }

        // ƒeƒXƒg
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) Execute();
        }

        public void Execute()
        {
            if (_effect.gameObject.activeSelf) return;

            _effect.gameObject.SetActive(true);
            _effect.Play();
        }
    }
}
