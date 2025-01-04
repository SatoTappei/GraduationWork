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
            // 名前,冒険結果,サブゴール,アイテム最大3個,出会った冒険者最大3人。
            string[] result = new string[9];

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

            // サブゴール。"ダンジョンの入口に戻る。"以外でランダムに1つ選ぶ。
            if (TryGetComponent(out SubGoalPath path) && path.Path != null && path.Path.Count > 0)
            {
                string[] subGoals = path.Path
                    .Select(a => a.Description.Japanese)
                    .Where(s => s != "ダンジョンの入口に戻る。")
                    .ToArray();

                result[2] = subGoals[Random.Range(0, subGoals.Length)];
            }

            // アイテム。送信する数は3つまで。
            if (TryGetComponent(out ItemInventory item))
            {
                string[] items = item.GetEntries().Select(e => e.Name).ToArray();
                for (int i = 0; i < Mathf.Min(items.Length, 3); i++)
                {
                    result[3 + i] = items[i];
                }
            }

            // 会話した冒険者。送信する数は3人まで。
            string[] talk = adventurer.Status.TalkRecord.Record.ToArray();
            for (int i = 0; i < Mathf.Min(talk.Length, 3); i++)
            {
                result[6 + i] = talk[i];
            }

            // 送信。
            string csv = string.Join(",", result);
            GameManager.ReportAdventureResult(adventurer, csv);
        }
    }
}
