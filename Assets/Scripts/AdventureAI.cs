using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // キャラクターシートから登場から脱出までの長期的なプランを作成。
    //  宝箱をn個取得する、○○ターン居座る、敵をn体倒す、○○を探す、などを組み合わせる。
    // 短期的な行動を達成しようとする。
    public class AdventureAI : MonoBehaviour
    {
        // ここがAIの判断に置きかわる。
        public async UniTask<string> SelectNextActionAsync()
        {
            return await RandomNextActionAsync();
        }

        // 行動の結果を報告。
        public void ReportActionResult(string result)
        {

        }

        // キー入力で手動制御する場合。
        async UniTask<string> InputNextActionAsync()
        {
            await UniTask.WaitUntil(() => Input.anyKey);

            if (Input.GetKeyDown(KeyCode.Alpha1)) return NumberToActionName(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) return NumberToActionName(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) return NumberToActionName(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) return NumberToActionName(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) return NumberToActionName(4);
            if (Input.GetKeyDown(KeyCode.Alpha6)) return NumberToActionName(5);
            else return string.Empty;
        }

        // ランダムな行動を選択する場合。
        async UniTask<string> RandomNextActionAsync()
        {
            await UniTask.Yield(); // 待つ必要ないが警告が出るので一応。
            return NumberToActionName(Random.Range(0, 5));
        }

        string NumberToActionName(int number)
        {
            if (number == 0) return "Move Treasure";
            if (number == 1) return "Move Enemy";
            if (number == 2) return "Move Entrance";
            if (number == 3) return "Interact Attack";
            if (number == 4) return "Interact Scav";
            if (number == 5) return "Interact Talk";
            else return string.Empty;
        }
    }
}