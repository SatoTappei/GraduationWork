using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using AI;

namespace Game
{
    public class SubGoalPath : MonoBehaviour
    {
        IReadOnlyList<SubGoal> _path;
        int _currentIndex;

        public SubGoal Current => _currentIndex < _path.Count ? _path[_currentIndex] : null;
        public bool IsLast => _currentIndex == _path.Count - 1;

        void Awake()
        {
            _path = new List<SubGoal>();
        }

        // AIにサブゴールを設定してもらう。
        // これ以外のメソッドはこのメソッドでサブゴールを設定した後に呼び出す想定。
        public async UniTask PlanningAsync(CancellationToken token)
        {
            await CreatePath(token);

            _currentIndex = 0;
        }

        public void HeadingNext()
        {
            _currentIndex++;
            _currentIndex = Mathf.Min(_currentIndex, _path.Count - 1);
        }

        async UniTask CreatePath(CancellationToken token)
        {
            // AIにキャラクターとしてのロールを与える。
            Blackboard blackboard = GetComponent<Blackboard>();
            string age = blackboard.AdventurerSheet.Age;
            string job = blackboard.AdventurerSheet.Job;
            string background = blackboard.AdventurerSheet.Background;
            string rolePrompt =
                $"# 指示内容\n" +
                $"- 以下のキャラクターになりきって各質問に答えてください。\n" +
                $"'''\n" +
                $"# キャラクター\n" +
                $"- {age}歳の{job}。\n" +
                $"- {background}\n";

            AIClient ai = new AIClient(rolePrompt);

            // AIにサブゴールを決めるようリクエスト。
            string prompt =
                $"# 指示内容\n" +
                $"- キャラクターを冒険者としてダンジョン探索ゲームに登場させます。\n" +
                $"- 自身のキャラクターの設定を基に、ゲームクリアまでに必要なサブゴールを選択してください。\n" +
                $"- 以下の選択肢から合計3つ選択してください。\n" +
                $"- ダンジョンから脱出するために、3つめは「ダンジョンの入口に戻る。」を選ぶことを推薦します。\n" +
                $"'''\n" +
                $"# 選択肢\n" +
                $"- お宝を手に入れる。 0\n" +
                $"- 依頼されたアイテムを手に入れる。 1\n" +
                $"- ダンジョン内を探索する。 2\n" +
                $"- 自分より弱そうな敵を倒す。 3\n" +
                $"- 強力な敵を倒す。 4\n" +
                $"- 他の冒険者を倒す。 5\n" +
                $"- ダンジョンの入口に戻る。 6\n" +
                $"'''\n" +
                $"# 出力形式\n" +
#if true
                $"- 各選択肢の末尾の番号のみを半角スペース区切りで出力してください。\n" +
#else
                // キャラクターの背景と照らし合わせて確認する用途。
                $"- 各選択肢の末尾の番号と、その選択をした理由を出力してください。\n" +
#endif
                $"'''\n" +
                $"# 出力例\n" +
                $"- 1 3 6\n" +
                $"- 4 5 6\n";

            string response = await ai.RequestAsync(prompt, token);

            // AIからのレスポンスが出力例とは異なる場合を想定し、文字列から数字のみを抽出する。
            List<string> result = response.Split().Where(s => int.TryParse(s, out int _)).ToList();

            // AIの出力方法が正常な場合、AIが選択した3つの番号のみが配列に格納されている。
            // それ以外の出力をした場合、宝箱入手 -> 探索 -> 出口へ戻る の順でサブゴールを指定。
            if (result.Count != 3)
            {
                Debug.LogError($"適切な数のサブゴールが設定されていない。: {string.Join(",", result)}");
                result = new List<string> { "0", "2", "6" };
            }

            // 対応するサブゴールのクラスに変換。
            SubGoal[] path = new SubGoal[result.Count];
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] == "0") path[i] = gameObject.AddComponent<GetTreasure>();
                else if (result[i] == "1") path[i] = gameObject.AddComponent<GetRequestedItem>();
                else if (result[i] == "2") path[i] = gameObject.AddComponent<ExploreDungeon>();
                else if (result[i] == "3") path[i] = gameObject.AddComponent<DefeatWeakEnemy>();
                else if (result[i] == "4") path[i] = gameObject.AddComponent<DefeatStrongEnemy>();
                else if (result[i] == "5") path[i] = gameObject.AddComponent<DefeatAdventurer>();
                else if (result[i] == "6") path[i] = gameObject.AddComponent<ReturnToEntrance>();
                else Debug.LogError($"AIが選択したサブゴールに対応するクラスが無い。: {result[i]}");
            }

            // AIが選択した番号に対応するサブゴールが無かった場合。
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] != null) continue;

                if (i == path.Length - 1) path[i] = gameObject.AddComponent<ReturnToEntrance>();
                else path[i] = gameObject.AddComponent<ExploreDungeon>();
            }

            _path = path;
        }
    }
}
