using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AdventurerAI : MonoBehaviour
    {
        Adventurer _adventurer;
        RolePlayAI _rolePlayAI;
        GamePlayAI _gamePlayAI;
        InformationEvaluateAI _informationEvaluateAI;
        TalkContentSelectAI _talkContentSelectAI;

        bool _isInitialized;
        string _selectedAction;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _rolePlayAI = new RolePlayAI(_adventurer);
            _gamePlayAI = new GamePlayAI(_adventurer);
            _informationEvaluateAI = new InformationEvaluateAI();
            _talkContentSelectAI = new TalkContentSelectAI();
        }

        void Start()
        {
            _isInitialized = true;
        }

        // �L�����N�^�[�̐ݒ���l�����ăT�u�S�[����I������悤�AAI�Ƀ��N�G�X�g���Č��ʂ�Ԃ��B
        public async UniTask<SubGoal[]> SelectSubGoalAsync()
        {
            await WaitInitializeAsync();

            IReadOnlyList<string> result = await _rolePlayAI.RequestSubGoalAsync();

            SubGoal[] subGoals = new SubGoal[result.Count];
            for (int i = 0; i < result.Count; i++)
            {
                subGoals[i] = Convert(result[i]);
            }

            return subGoals;

            // AI�͓��{��̕�����őI�������T�u�S�[����Ԃ��̂ŁA�Ή�����N���X�̃C���X�^���X�ɕϊ�����B
            SubGoal Convert(string text)
            {
                if (text == GetTreasure.StaticText.Japanese) return new GetTreasure(_adventurer);
                if (text == GetRequestedItem.StaticText.Japanese) return new GetRequestedItem(_adventurer);
                if (text == ExploreDungeon.StaticText.Japanese) return new ExploreDungeon(_adventurer);
                if (text == DefeatWeakEnemy.StaticText.Japanese) return new DefeatWeakEnemy(_adventurer);
                if (text == DefeatStrongEnemy.StaticText.Japanese) return new DefeatStrongEnemy(_adventurer);
                if (text == DefeatAdventurer.StaticText.Japanese) return new DefeatAdventurer(_adventurer);
                if (text == ReturnToEntrance.StaticText.Japanese) return new ReturnToEntrance(_adventurer);

                Debug.LogError($"AI���I�������T�u�S�[���ɑΉ�����N���X�������B: {text}");

                return null;
            }
        }

        // AI�Ɏ��̍s����I��������B
        public async UniTask<string> SelectNextActionAsync()
        {
            await WaitInitializeAsync();
            string response = await _gamePlayAI.RequestNextActionAsync();

#if UNITY_EDITOR
            _selectedAction = response;
#endif
            return response;
        }

        // ���e�Ə�񌹂���ɁAAI�ɏ��̐M���x��]��������B
        public async UniTask<float> EvaluateInformationAsync(SharedInformation information)
        {
            return await _informationEvaluateAI.EvaluateAsync(information);
        }

        // ���g�̒m���Ă�����̂����A���̖`���҂ɒ�����̂�AI�ɑI��������B
        public async UniTask<SharedInformation> SelectTalkContentAsync(IReadOnlyList<SharedInformation> information)
        {
            return await _talkContentSelectAI.SelectAsync(information);
        }

        async UniTask WaitInitializeAsync()
        {
            await UniTask.WaitUntil(() => _isInitialized);
        }
    }
}