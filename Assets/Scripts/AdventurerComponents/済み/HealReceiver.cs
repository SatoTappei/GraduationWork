using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HealReceiver : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particle;

        Blackboard _blackboard;
        StatusBarBinder _statusBar;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _statusBar = GetComponent<StatusBarBinder>();
        }

        public void Heal(int value, Vector2Int coords)
        {
            // oðÄ¶B
            if (_particle != null)
            {
                _particle.Play();
            }

            // ÌÍÉ½fB
            _blackboard.CurrentHp += value;
            _blackboard.CurrentHp = Mathf.Min(_blackboard.CurrentHp, _blackboard.MaxHp);

            // UIÉ½fB
            _statusBar.Apply();
        }
    }
}
