using AI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    public static class SubGoalSelector
    {
        public static async UniTask<IReadOnlyList<string>> SelectAsync(AdventurerSheet sheet, CancellationToken token)
        {
            string rolePrompt;
            if (sheet == null)
            {
                Debug.LogWarning("�T�u�S�[�������߂�̂ɕK�v�Ȗ`���҃f�[�^�������B");

                rolePrompt = "�Q�[���̃L�����N�^�[�Ƃ��ĐU�镑���A�e����ɓ����Ă��������B";
            }
            else
            {
                rolePrompt =
                    $"# �w�����e\n" +
                    $"- �ȉ��̃L�����N�^�[�ɂȂ肫���Ċe����ɓ����Ă��������B\n" +
                    $"# �L�����N�^�[\n" +
                    $"- {sheet.Age}�΂�{sheet.Job}\n" +
                    $"- {sheet.Personality}\n" +
                    $"- {sheet.Motivation}\n" +
                    $"- {sheet.Weaknesses}\n" +
                    $"- {sheet.Background}\n";
            }
            AIClient ai = new AIClient(rolePrompt);

            // �T�u�S�[���͍��v2�B�Ō�͕K���u�����ɖ߂�v�Ȃ̂ŁA1�I��ł��炤�B
            // ���l�ŏo�͂����Ă��邪�A������ŏo�͂�����悤���ǂ��������ǂ���������Ȃ��B
            string prompt =
                $"# �w�����e\n" +
                $"- �L�����N�^�[��`���҂Ƃ��ă_���W�����T���Q�[���ɓo�ꂳ���܂��B\n" +
                $"- ���g�̃L�����N�^�[�̐ݒ����ɁA�Q�[���N���A�܂łɕK�v�ȃT�u�S�[����I�����Ă��������B\n" +
                $"- �ȉ��̑I��������1�I�����Ă��������B\n" +
                $"# �I����\n" +
                $"- �������ɓ���� 0\n" +
                $"- �˗����ꂽ�A�C�e������ɓ���� 1\n" +
                $"- �˗����ꂽ�G��|�� 2\n" +
                $"- �_���W�����̃{�X��|�� 3\n" +
                $"- ���̖`���҂�|�� 4\n" +
#if true
                $"- �e�I�����̖����̔ԍ��݂̂��o�͂��Ă��������B";
#else
                // �L�����N�^�[�̔w�i�ƏƂ炵���킹�Ċm�F����p�r�B
                $"- �e�I�����̖����̔ԍ��ƁA���̑I�����������R���o�͂��Ă��������B";
#endif

            string response = await ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();

            // AI����̃��X�|���X���o�͗�Ƃ͈قȂ�ꍇ��z�肵�A�����񂩂琔���݂̂𒊏o����B
            List<int> result =
                response
                .Split()
                .Where(s => int.TryParse(s, out int _))
                .Select(s => int.Parse(s))
                .ToList();

            // �I�񂾔ԍ��ɑΉ������T�u�S�[���������ꍇ�A0�Ԃ́u�������ɓ����v�ɒu������B
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] < 0 || 4 < result[i])
                {
                    result[i] = 0;
                }
            }

            // �I�񂾃T�u�S�[���̐���0�̏ꍇ�A0�Ԃ́u�������ɓ����v�Ŗ��߂�B
            if (result.Count == 0)
            {
                result.Add(0);
            }

            // �I�񂾃T�u�S�[���̐���2�ȏ�̏ꍇ�A�擪1�̂ݑI�������B
            result = result.Take(1).ToList();

            // �ԍ��ɑΉ�����T�u�S�[�����ɍēx�ϊ��B
            List<string> subGoals = new List<string>();
            foreach (int n in result)
            {
                if (n == 0) subGoals.Add("�������ɓ����");
                else if (n == 1) subGoals.Add("�˗����ꂽ�A�C�e������ɓ����");
                else if (n == 2) subGoals.Add("�˗����ꂽ�G��|��");
                else if (n == 3) subGoals.Add("�_���W�����̃{�X��|��");
                else if (n == 4) subGoals.Add("���̖`���҂�|��");
                else subGoals.Add("�������ɓ����");
            }

            // 2�߂̃T�u�S�[���͑S�L�����N�^�[���ʂŁu�_���W�����̓����ɖ߂�v�ɂȂ�B
            subGoals.Add("�_���W�����̓����ɖ߂�");

            return subGoals;
        }
    }
}