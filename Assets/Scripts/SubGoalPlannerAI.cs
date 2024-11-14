using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class SubGoalPlannerAI
    {
        AIRequest _ai;

        public SubGoalPlannerAI(IReadOnlyAdventurerContext context)
        {
            // �L�����N�^�[�Ƃ��ĐU�镑��AI�͑䎌��w�i�Ȃǂ�UI�ɕ\������̂œ��{��B
            string age = context.AdventurerSheet.Age;
            string job = context.AdventurerSheet.Job;
            string background = context.AdventurerSheet.Background;
            string prompt =
                $"# �w�����e\n" +
                $"- �ȉ��̃L�����N�^�[�ɂȂ肫���Ċe����ɓ����Ă��������B\n" +
                $"'''\n" +
                $"# �L�����N�^�[\n" +
                $"- {age}�΂�{job}�B\n" +
                $"- {background}\n";

            _ai = AIRequestFactory.Create(prompt);
        }

        public async UniTask<IReadOnlyList<string>> RequestAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            string prompt =
                $"# �w�����e\n" +
                $"- �L�����N�^�[��`���҂Ƃ��ă_���W�����T���Q�[���ɓo�ꂳ���܂��B\n" +
                $"- ���g�̃L�����N�^�[�̐ݒ����ɁA�Q�[���N���A�܂łɕK�v�ȃT�u�S�[����I�����Ă��������B\n" +
                $"- �ȉ��̑I�������獇�v3�I�����Ă��������B\n" +
                $"- �_���W��������E�o���邽�߂ɁA3�߂́u{ReturnToEntrance.StaticText.Japanese}�v��I�Ԃ��Ƃ𐄑E���܂��B\n" +
                $"'''\n" +
                $"# �I����\n" +
                $"- {GetTreasure.StaticText.Japanese} 0\n" +
                $"- {GetRequestedItem.StaticText.Japanese} 1\n" +
                $"- {ExploreDungeon.StaticText.Japanese} 2\n" +
                $"- {DefeatWeakEnemy.StaticText.Japanese} 3\n" +
                $"- {DefeatStrongEnemy.StaticText.Japanese} 4\n" +
                $"- {DefeatAdventurer.StaticText.Japanese} 5\n" +
                $"- {ReturnToEntrance.StaticText.Japanese} 6\n" +
                $"'''\n" +
                $"# �o�͌`��\n" +
#if true
                $"- �e�I�����̖����̔ԍ��݂̂𔼊p�X�y�[�X��؂�ŏo�͂��Ă��������B\n" +
#else
                // �L�����N�^�[�̔w�i�ƏƂ炵���킹�Ċm�F����p�r�B
                $"- �e�I�����̖����̔ԍ��ƁA���̑I�����������R���o�͂��Ă��������B\n" +
#endif
                $"'''\n" +
                $"# �o�͗�\n" +
                $"- 1 3 6\n" +
                $"- 4 5 6\n";

            string response = await _ai.RequestAsync(prompt);

            // AI����̃��X�|���X���o�͗�Ƃ͈قȂ�ꍇ��z�肵�A�����񂩂琔���݂̂𒊏o����B
            List<string> result = response.Split().Where(s => int.TryParse(s, out int _)).ToList();

            // AI�̏o�͕��@������ȏꍇ�AAI���I������3�̔ԍ��݂̂��z��Ɋi�[����Ă���B
            if (result.Count != 3)
            {
                Debug.LogError($"�K�؂Ȑ��̃T�u�S�[�����ݒ肳��Ă��Ȃ��B: {string.Join(",", result)}");
            }

            // ���������̂܂ܕԂ���AI������I�񂾂̂��Ăяo�����Ŕc�����h���B
            // ���̑΍�Ƃ��āA������Ή�����T�u�S�[�����ɕϊ����ĕԂ��B
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] == "0") result[i] = GetTreasure.StaticText.Japanese;
                else if (result[i] == "1") result[i] = GetRequestedItem.StaticText.Japanese;
                else if (result[i] == "2") result[i] = ExploreDungeon.StaticText.Japanese;
                else if (result[i] == "3") result[i] = DefeatWeakEnemy.StaticText.Japanese;
                else if (result[i] == "4") result[i] = DefeatStrongEnemy.StaticText.Japanese;
                else if (result[i] == "5") result[i] = DefeatAdventurer.StaticText.Japanese;
                else if (result[i] == "6") result[i] = ReturnToEntrance.StaticText.Japanese;
            }

            return result;
        }
    }
}
