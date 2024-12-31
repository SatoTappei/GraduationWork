using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EntryToDungeon : BaseAction
    {
        // 台詞を表示するので、ステータスバーに登録した後に呼ぶ想定。
        public void Entry()
        {
            // 登場時の演出。
            if (TryGetComponent(out EntryEffect entryEffect)) entryEffect.Play();

            // 登場時の台詞。
            if (TryGetComponent(out LineApply line)) line.ShowLine(RequestLineType.Entry);

            // ゲーム進行ログに表示。
            Blackboard blackboard = GetComponent<Blackboard>();
            GameLog.Add($"システム", $"{blackboard.DisplayName}がダンジョンにやってきた。", GameLogColor.White);
        }
    }
}
