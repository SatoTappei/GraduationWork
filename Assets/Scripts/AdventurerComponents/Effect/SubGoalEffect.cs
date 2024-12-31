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
                string log = $"{blackboard.DisplayName}���u{subGoalPath.GetCurrent().Text.Japanese}�v��B���B";
                GameLog.Add($"�V�X�e��", log, GameLogColor.White);
            }
        }
    }
}
