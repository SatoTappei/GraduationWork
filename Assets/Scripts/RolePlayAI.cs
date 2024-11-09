using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    public enum RequestLineType
    {
        Entry,              // �o��B
        Defeated,           // ���j���ꂽ�B
        Goal,               // �E�o�����B
        GetTreasureSuccess, // �󕨂����B
        GetTreasureFailure, // �󔠂�����ہB
        GetItemSuccess,     // �A�C�e�������B
        GetItemFailure,     // �A�C�e�������������B
        DefeatEnemy,        // �G�����j�����B
        Attack,             // �U�����̊|�����B
        Damage,             // �_���[�W���󂯂��Ƃ��̙���B
        Greeting            // ���̖`���҂ɐ��������鎞�̈��A�B
    }

    public class RolePlayAI
    {
        AIRequest _ai;

        public RolePlayAI(IRolePlayAIResource resource)
        {
            // �L�����N�^�[�Ƃ��ĐU�镑��AI�͑䎌��w�i�Ȃǂ�UI�ɕ\������̂œ��{��B
            string age = resource.AdventurerSheet.Age;
            string job = resource.AdventurerSheet.Job;
            string background = resource.AdventurerSheet.Background;
            string prompt =
                $"# �w�����e\n" +
                $"- �ȉ��̃L�����N�^�[�ɂȂ肫���Ċe����ɓ����Ă��������B\n" +
                $"'''\n" +
                $"# �L�����N�^�[\n" +
                $"- {age}�΂�{job}�B\n" +
                $"- {background}\n";

            _ai = AIRequestFactory.Create(prompt);
        }

        public async UniTask<IReadOnlyList<string>> RequestSubGoalAsync(CancellationToken token)
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

        public async UniTask<string> RequestLineAsync(RequestLineType type, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            (string content, string sample) instruction = GetContentAndSample(type);

            string prompt =
                $"# �w�����e\n" +
                $"- ���g�̃L�����N�^�[�̐ݒ����ɁA{instruction.content}���l���Ă��������B\n" +
                $"- �Z���ꌾ�ő䎌�����肢���܂��B\n" +
                $"'''\n" +
                $"# �o�͌`��\n" +
                $"- �䎌�݂̂����̂܂܏o�͂��Ă��������B\n" +
                $"'''\n" +
                $"# �o�͗�\n" +
                $"- {instruction.sample}\n";
            string response = await _ai.RequestAsync(prompt);

            if (response[0] == '-')
            {
                response = response[1..];
            }

            return response.Trim().Trim('�u', '�v');
        }

        static (string, string) GetContentAndSample(RequestLineType type)
        {
            if (type == RequestLineType.Entry) return ("�o�ꎞ�̑䎌", "�撣�邼�I");
            if (type == RequestLineType.Defeated) return ("�G�Ƃ̃o�g���Ŕs�k�����ۂ̑䎌", "�������߂�");
            if (type == RequestLineType.Goal) return ("�Q�[�����N���A�����ۂ̑䎌", "�΂��΂�");
            if (type == RequestLineType.GetTreasureSuccess) return ("�󕨂���肵���ۂ̑䎌", "������[���I");
            if (type == RequestLineType.GetTreasureFailure) return ("�󔠂̒��g������ۂŎc�O�������ۂ̑䎌", "��������");
            if (type == RequestLineType.GetItemSuccess) return ("�A�C�e������肵���ۂ̑䎌", "�������");
            if (type == RequestLineType.GetItemFailure) return ("�A�C�e����T���������������ꍇ�̑䎌", "�����Ȃ���");
            if (type == RequestLineType.DefeatEnemy) return ("�G�����j�����ۂ̑䎌", "�������I");
            if (type == RequestLineType.Attack) return ("�G���U������ۂ̊|����", "���Ⴀ���I");
            if (type == RequestLineType.Damage) return ("�U������A�_���[�W���󂯂��ۂ̑䎌", "�����I");
            if (type == RequestLineType.Greeting) return ("���̐l�ɐ���������ۂ̈��A", "�˂��˂�");

            return default;
        }
    }
}
