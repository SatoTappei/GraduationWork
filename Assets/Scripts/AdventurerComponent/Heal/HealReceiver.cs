using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HealReceiver : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particle;

        Adventurer _adventurer;
        StatusBarBinder _statusBar;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _statusBar = GetComponent<StatusBarBinder>();
        }

        public void Heal(int value, Vector2Int coords)
        {
            // 演出を再生。
            if (_particle != null)
            {
                _particle.Play();
            }

            // 体力に反映。
            _adventurer.Status.CurrentHp += value;

            // UIに反映。
            _statusBar.Apply();
        }
    }
}
