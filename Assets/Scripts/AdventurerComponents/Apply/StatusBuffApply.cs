using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatusBuffApply : MonoBehaviour
    {
        Blackboard _blackboard;
        WaitForSeconds _waitDuration;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Buff(1.1f, 1.3f);
            }
        }

        public void Buff(float attack, float speed)
        {
            StartCoroutine(BuffAsync(attack, speed));
        }

        IEnumerator BuffAsync(float attack, float speed)
        {
            const float Duration = 30.0f;

            if (TryGetComponent(out StatusBuffEffect effect)) effect.Play(Duration);

            _blackboard.AttackMagnification = attack;
            _blackboard.SpeedMagnification = speed;

            yield return _waitDuration ??= new WaitForSeconds(Duration);

            _blackboard.AttackMagnification = 1.0f;
            _blackboard.SpeedMagnification = 1.0f;
        }
    }
}
