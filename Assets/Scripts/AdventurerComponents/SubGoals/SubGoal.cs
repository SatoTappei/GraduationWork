using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // 各サブゴールのクラスはこのクラスを継承する。
    // AIが選んだサブゴールに対応するクラスをAddComponentする想定。
    public abstract class SubGoal : MonoBehaviour
    {
        public abstract BilingualString Text { get; }

        public abstract bool IsCompleted();
        public virtual IEnumerable<string> GetAdditionalActions() { yield break; }
    }
}
