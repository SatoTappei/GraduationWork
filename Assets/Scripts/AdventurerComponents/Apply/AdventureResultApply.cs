using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    // GAS側でネストされたJSONのパースが上手くいかないので、CSV形式で送信する。
    public class AdventureResultApply : MonoBehaviour
    {
        public void Send()
        {
            // 名前,冒険結果,サブゴール最大3個,アイテム最大3個。
            string[] result = new string[8];

            // 名前。
            TryGetComponent(out Adventurer adventurer);
            result[0] = adventurer.AdventurerSheet.FullName;

            // 冒険結果、脱出した場合は一定確率で引退、撃破された場合は一定確率で死亡。
            TryGetComponent(out Blackboard blackboard);
            if (blackboard.CurrentHp > 0)
            {
                if (Random.value <= 1.0f) result[1] = "Escape";
                else result[1] = "Retire";
            }
            else
            {
                if (Random.value <= 1.0f) result[1] = "Defeated";
                else result[1] = "Rescue";
            }

            // サブゴール。
            if (TryGetComponent(out SubGoalPath path) && path.Path != null && path.Path.Count > 0)
            {
                string[] subGoals = path.Path.Select(a => a.Text.Japanese).ToArray();
                for (int i = 0; i < subGoals.Length; i++)
                {
                    result[2 + i] = subGoals[i];
                }
            }

            // アイテム。
            if (TryGetComponent(out ItemInventory item))
            {
                string[] items = item.GetEntries().Select(e => e.Name).ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    result[5 + i] = items[i];
                }
            }

            // 送信。
            string csv = string.Join(",", result);
            GameManager.ReportAdventureResult(adventurer, csv);
        }
    }
}
