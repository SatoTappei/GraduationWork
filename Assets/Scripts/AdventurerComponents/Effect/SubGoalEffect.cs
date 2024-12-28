using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class SubGoalEffect : MonoBehaviour
    {
        public void Play()
        {
            if (GameLog.TryFind(out GameLog gameLog) && TryGetComponent(out SubGoalPath subGoalPath))
            {
                Blackboard blackboard = GetComponent<Blackboard>();
                string log = $"{blackboard.DisplayName}が「{subGoalPath.Current.Text.Japanese}」を達成。";
                gameLog.Add($"システム", log, GameLogColor.White);
            }
        }
    }
}
