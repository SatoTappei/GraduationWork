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
                ui.AddLog($"{blackboard.DisplayName}が「{subGoalPath.Current.Text.Japanese}」を達成。");
            }
        }
    }
}
