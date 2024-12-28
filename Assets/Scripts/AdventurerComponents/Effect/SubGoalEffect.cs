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
                string log = $"{blackboard.DisplayName}���u{subGoalPath.Current.Text.Japanese}�v��B���B";
                gameLog.Add($"�V�X�e��", log, GameLogColor.White);
            }
        }
    }
}
