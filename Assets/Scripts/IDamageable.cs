using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IDamageable
    {
        // 死亡や被ダメ―ジ無効化など、ダメージを与えた結果を返せるようにしておく。
        public string Damage(int value, Vector2Int coords, string effect = "");
    }
}
