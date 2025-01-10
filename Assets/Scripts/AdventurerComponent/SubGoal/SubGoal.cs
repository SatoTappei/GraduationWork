using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class SubGoal : MonoBehaviour
    {
        public enum State
        {
            Pending,   // 実行待ち。
            Running,   // 実行中。
            Completed, // 完了条件を満たした。
            Failed,    // 何らかの原因で失敗した。
        }

        // UIへの表示の他、AIへのリクエストにも使うので、日本語と英文の両方が必要。
        public abstract BilingualString Description { get; }

        public abstract State Check();
    }
}
