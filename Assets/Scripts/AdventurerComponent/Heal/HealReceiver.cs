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
            // ���o���Đ��B
            if (_particle != null)
            {
                _particle.Play();
            }

            // �̗͂ɔ��f�B
            _adventurer.Status.CurrentHp += value;

            // UI�ɔ��f�B
            _statusBar.Apply();
        }
    }
}
