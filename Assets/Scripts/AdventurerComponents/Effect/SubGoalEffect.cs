using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class SubGoalEffect : MonoBehaviour
    {
        public void Play()
        {
            if (TryGetComponent(out SubGoalPath subGoalPath))
            {
                Blackboard blackboard = GetComponent<Blackboard>();
                string log = $"{blackboard.DisplayName}が「{subGoalPath.GetCurrent().Text.Japanese}」を達成。";
                GameLog.Add($"システム", log, GameLogColor.White);
            }
        }
    }
}
