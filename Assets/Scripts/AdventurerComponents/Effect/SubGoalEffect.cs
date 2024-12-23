using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class SubGoalEffect : MonoBehaviour
    {
        public void Play()
        {
            if (UiManager.TryFind(out UiManager ui) && TryGetComponent(out SubGoalPath subGoalPath))
            {
                Blackboard blackboard = GetComponent<Blackboard>();
                string log = $"{blackboard.DisplayName}���u{subGoalPath.Current.Text.Japanese}�v��B���B";
                ui.AddLog($"�V�X�e��", log, GameLogColor.White);
            }
        }
    }
}
