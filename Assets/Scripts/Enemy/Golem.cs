using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Golem : Enemy
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetComponent<IDamageable>().Damage(10000, default);
            }
        }
    }
}
