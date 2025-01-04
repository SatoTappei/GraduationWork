using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class SubGoal : MonoBehaviour
    {
        // 説明文。UIへの表示の他、AIへのリクエストにも使うので、日本語と英文の両方が必要。
        public abstract BilingualString Description { get; }

        // 完了条件を満たしたかどうか。
        public abstract bool IsCompleted();
    }
}
