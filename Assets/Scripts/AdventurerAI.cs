using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class AdventurerAI : MonoBehaviour
    {
        Adventurer _adventurer;
        RolePlayAI _rolePlayAI;
        GamePlayAI _gamePlayAI;
        ScoreEvaluateAI _scoreEvaluateAI;
        TurnEvaluateAI _turnEvaluateAI;
        TalkContentSelectAI _talkContentSelectAI;

        bool _isInitialized;
        string _selectedAction;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _rolePlayAI = new RolePlayAI(_adventurer);
            _gamePlayAI = new GamePlayAI(_adventurer);
            _scoreEvaluateAI = new ScoreEvaluateAI();
            _turnEvaluateAI = new TurnEvaluateAI();
            _talkContentSelectAI = new TalkContentSelectAI();
        }

        void Start()
        {
            _isInitialized = true;
        }

        // キャラクターの設定を考慮してサブゴールを選択するよう、AIにリクエストして結果を返す。
        public async UniTask<SubGoal[]> SelectSubGoalAsync(CancellationToken token)
        {
            await WaitInitializeAsync(token);

            IReadOnlyList<string> result = await _rolePlayAI.RequestSubGoalAsync(token);

            SubGoal[] subGoals = new SubGoal[result.Count];
            for (int i = 0; i < result.Count; i++)
            {
                subGoals[i] = Convert(result[i]);
            }

            return subGoals;

            // AIは日本語の文字列で選択したサブゴールを返すので、対応するクラスのインスタンスに変換する。
            SubGoal Convert(string text)
            {
                if (text == GetTreasure.StaticText.Japanese) return new GetTreasure(_adventurer);
                if (text == GetRequestedItem.StaticText.Japanese) return new GetRequestedItem(_adventurer);
                if (text == ExploreDungeon.StaticText.Japanese) return new ExploreDungeon(_adventurer);
                if (text == DefeatWeakEnemy.StaticText.Japanese) return new DefeatWeakEnemy(_adventurer);
                if (text == DefeatStrongEnemy.StaticText.Japanese) return new DefeatStrongEnemy(_adventurer);
                if (text == DefeatAdventurer.StaticText.Japanese) return new DefeatAdventurer(_adventurer);
                if (text == ReturnToEntrance.StaticText.Japanese) return new ReturnToEntrance(_adventurer);

                Debug.LogError($"AIが選択したサブゴールに対応するクラスが無い。: {text}");

                return null;
            }
        }

        // AIに次の行動を選択させる。
        public async UniTask<string> SelectNextActionAsync(CancellationToken token)
        {
            await WaitInitializeAsync(token);
            string response = await _gamePlayAI.RequestNextActionAsync();

#if UNITY_EDITOR
            _selectedAction = response;
#endif
            return response;
        }

        // 内容と情報源を基に、AIに情報のスコアを評価させる。
        public async UniTask<float> EvaluateScoreAsync(SharedInformation information, CancellationToken token)
        {
            return await _scoreEvaluateAI.EvaluateAsync(information, token);
        }

        // 内容を基に、AIに情報の有効ターンを評価させる。
        public async UniTask<int> EvaluateTurnAsync(SharedInformation information, CancellationToken token)
        {
            return await _turnEvaluateAI.EvaluateAsync(information, token);
        }

        // 自身の知っている情報のうち、他の冒険者に喋るものをAIに選択させる。
        public async UniTask<SharedInformation> SelectInformationAsync(IReadOnlyList<SharedInformation> information, CancellationToken token)
        {
            return await _talkContentSelectAI.SelectAsync(information, token);
        }

        public async UniTask<string> RequestLineAsync(RequestLineType type, CancellationToken token)
        {
            return await _rolePlayAI.RequestLineAsync(type, token);
        }

        async UniTask WaitInitializeAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await UniTask.WaitUntil(() => _isInitialized);
        }
    }
}