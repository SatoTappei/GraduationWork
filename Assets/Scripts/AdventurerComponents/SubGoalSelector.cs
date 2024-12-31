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

            // �T�u�S�[���͍��v3�B�Ō�͕K���u�����ɖ߂�v�Ȃ̂ŁA2�I��ł��炤�B
            // ���l�ŏo�͂����Ă��邪�A������ŏo�͂�����悤���ǂ��������ǂ���������Ȃ��B
            string prompt =
                $"# �w�����e\n" +
                $"- �L�����N�^�[��`���҂Ƃ��ă_���W�����T���Q�[���ɓo�ꂳ���܂��B\n" +
                $"- ���g�̃L�����N�^�[�̐ݒ����ɁA�Q�[���N���A�܂łɕK�v�ȃT�u�S�[����I�����Ă��������B\n" +
                $"- �ȉ��̑I��������2�I�����Ă��������B\n" +
                $"# �I����\n" +
                $"- �������ɓ���� 0\n" +
                $"- �˗����ꂽ�A�C�e������ɓ���� 1\n" +
                $"- �_���W��������T������ 2\n" +
                $"- �������セ���ȓG��|�� 3\n" +
                $"- ���͂ȓG��|�� 4\n" +
                $"- ���̖`���҂�|�� 5\n" +
                $"# �o�͌`��\n" +
#if true
                $"- �e�I�����̖����̔ԍ��݂̂𔼊p�X�y�[�X��؂�ŏo�͂��Ă��������B\n" +
#else
                // �L�����N�^�[�̔w�i�ƏƂ炵���킹�Ċm�F����p�r�B
                $"- �e�I�����̖����̔ԍ��ƁA���̑I�����������R���o�͂��Ă��������B\n" +
#endif
                $"# �o�͗�\n" +
                $"- 1 3\n" +
                $"- 4 5\n";

            string response = await ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();

            // AI����̃��X�|���X���o�͗�Ƃ͈قȂ�ꍇ��z�肵�A�����񂩂琔���݂̂𒊏o����B
            List<int> result =
                response
                .Split()
                .Where(s => int.TryParse(s, out int _))
                .Select(s => int.Parse(s))
                .ToList();

            // �I�񂾔ԍ��ɑΉ������T�u�S�[���������ꍇ�A2�Ԃ́u�_���W��������T������v�ɒu������B
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] < 0 || 5 < result[i])
                {
                    result[i] = 2;
                }
            }

            // �I�񂾃T�u�S�[���̐���2�����̏ꍇ�A2�Ԃ́u�_���W��������T������v�Ŗ��߂�B
            if (result.Count < 2)
            {
                for (int i = 2 - result.Count; i >= 0; i--)
                {
                    result.Add(2);
                }
            }

            // �I�񂾃T�u�S�[���̐���3�ȏ�̏ꍇ�A�擪2�̂ݑI�������B
            result = result.Take(2).ToList();

            // �ԍ��ɑΉ�����T�u�S�[�����ɍēx�ϊ��B
            List<string> subGoals = new List<string>();
            foreach (int n in result)
            {
                if (n == 0) subGoals.Add("�������ɓ����");
                else if (n == 1) subGoals.Add("�˗����ꂽ�A�C�e������ɓ����");
                else if (n == 2) subGoals.Add("�_���W��������T������");
                else if (n == 3) subGoals.Add("�������セ���ȓG��|��");
                else if (n == 4) subGoals.Add("���͂ȓG��|��");
                else if (n == 5) subGoals.Add("���̖`���҂�|��");
                else subGoals.Add("�_���W��������T������");
            }

            // 3�߂̃T�u�S�[���͑S�L�����N�^�[���ʂŁu�_���W�����̓����ɖ߂�v�ɂȂ�B
            subGoals.Add("�_���W�����̓����ɖ߂�");

            return subGoals;
        }
    }
}