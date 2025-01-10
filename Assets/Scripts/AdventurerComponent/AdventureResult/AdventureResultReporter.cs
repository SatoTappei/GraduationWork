using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    // GAS側でネストされたJSONのパースが上手くいかないので、CSV形式で送信する。
    public class AdventureResultReporter : MonoBehaviour
    {
        public void Send()
        {
            // 名前,冒険結果,サブゴール,サブゴールの結果(達成/諦めた)。
            string[] result = new string[4];

            // 名前。
            TryGetComponent(out Adventurer adventurer);
            result[0] = adventurer.AdventurerSheet.FullName;

            // 冒険結果、脱出した場合は一定確率で引退、撃破された場合は一定確率で死亡。
            if (adventurer.Status.CurrentHp > 0)
            {
                if (Random.value <= 1.0f) result[1] = "Escape";
                else result[1] = "Retire";
            }
            else
            {
                if (Random.value <= 1.0f) result[1] = "Die";
                else result[1] = "Rescue";
            }

            // サブゴール。「ダンジョンの入口に戻る。」以外でランダムに1つ選ぶ。
            TryGetComponent(out SubGoalPath path);
            if (path.Path == null)
            {
                Debug.LogWarning("サブゴールがnullなので空欄のまま送信。");
            }
            else if (path.Path.Count == 0)
            {
                Debug.LogWarning("サブゴールが1つもないので空欄のまま送信。");
            }
            else
            {
                string[] subGoals = path.Path
                    .Select(a => a.Description.Japanese)
                    .Where(s => s != "ダンジョンの入口に戻る。")
                    .ToArray();

                result[2] = subGoals[Random.Range(0, subGoals.Length)];
            }

            if (path.Path[0].Check() == SubGoal.State.Completed)
            {
                result[3] = "Completed";
            }
            else if (path.Path[0].Check() == SubGoal.State.Failed)
            {
                result[3] = "Failed";
            }
            else
            {
                Debug.LogWarning($"サブゴールを終えていない。:{path.Path[0].Check()}");
                result[3] = "Failed";
            }

            // 送信。
            string csv = string.Join(",", result);
            Debug.Log(csv);
            GameManager.ReportAdventureResult(adventurer, csv);
        }
    }
}
