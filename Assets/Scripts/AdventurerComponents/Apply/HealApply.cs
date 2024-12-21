using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HealApply : MonoBehaviour
    {
        public void Heal(int value)
        {
            TryGetComponent(out Blackboard blackboard);

            // ‰ñ•œ‰‰o‚ğÄ¶B
            if (TryGetComponent(out HealEffect effect))
            {
                effect.Play();
            }

            blackboard.CurrentHp += value;
            blackboard.CurrentHp = Mathf.Min(blackboard.CurrentHp, blackboard.MaxHp);

            // UI‚É”½‰fB
            if (TryGetComponent(out StatusBarApply statusBar))
            {
                statusBar.Apply();
            }
        }
    }
}
