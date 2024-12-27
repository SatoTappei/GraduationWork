using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class CalculationFormula
    {
        // レベル1で100、レベル30で300。
        public static int GetHp(int level)
        {
            float f = (200.0f / 29) * level + (2900.0f / 29);
            return Mathf.FloorToInt(f) - 6;
        }

        // レベル1で35、レベル30で93。
        public static int GetAttack(int level)
        {
            return 33 + level * 2;
        }

        // レベル1で1、レベル30で2。
        public static float GetSpeed(int level)
        {
            float f = 1.0f + (level / 30.0f);
            return Mathf.Floor(f * 10.0f) / 10.0f;
        }
    }
}