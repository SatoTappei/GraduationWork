using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using AI;

namespace Game
{
    public enum RequestLineType
    {
        None,
        Entry,              // �o��B
        Defeated,           // ���j���ꂽ�B
        Goal,               // �E�o�����B
        GetArtifactSuccess, // �A�[�e�B�t�@�N�g�����B
        GetItemSuccess,     // �A�C�e�������B
        GetItemFailure,     // �A�C�e�������������B
        DefeatEnemy,        // �G�����j�����B
        Attack,             // �U�����̊|�����B
        Damage,             // �_���[�W���󂯂��Ƃ��̙���B
        Greeting            // ���̖`���҂ɐ��������鎞�̈��A�B
    }

    public class RolePlay : MonoBehaviour
    {
        AIClient _ai;

        public void Initialize()
        {
            TryGetComponent(out Adventurer adventurer);

            if (adventurer.Sheet == null)
            {
                Debug.LogWarning("�`���҂̃f�[�^���ǂݍ��܂�Ă��Ȃ��B");

                _ai = new AIClient("�K���ȃL�����N�^�[�ɂȂ肫���Ċe����ɓ����Ă��������B");
            }
            else
            {
                // �L�����N�^�[�Ƃ��ĐU�镑��AI�͑䎌��w�i�Ȃǂ�UI�ɕ\������̂œ��{��B
                string prompt =
                    $"# �w�����e\n" +
                    $"- �ȉ��̃L�����N�^�[�ɂȂ肫���Ċe����ɓ����Ă��������B\n" +
                    $"# �L�����N�^�[\n" +
                    $"- {adventurer.Sheet.Sex}\n" +
                    $"- {adventurer.Sheet.Age}\n" +
                    $"- {adventurer.Sheet.Job}\n" +
                    $"- {adventurer.Sheet.Background}";

                _ai = new AIClient(prompt);
            }
        }

        public async UniTask<string> RequestLineAsync(RequestLineType type, CancellationToken token)
        {
            // ���������ꂸ�ɌĂ΂ꂽ�ꍇ�B
            if (_ai == null)
            {
                Debug.LogWarning("�����������ɑ䎌�����N�G�X�g�����̂ŁA���N�G�X�g�O�ɏ����������B");
                Initialize();
            }

            (string lineType, string sample) instruction = GetInstruction(type);
#if true
            string prompt =
                $"# �w�����e\n" +
                $"- ���g�̃L�����N�^�[�̐ݒ����ɁA{instruction.lineType}���l���Ă��������B\n" +
                $"- �Z���ꌾ�ő䎌�����肢���܂��B\n" +
                $"# �o�͌`��\n" +
                $"- �䎌�݂̂����̂܂܏o�͂��Ă��������B\n" +
                $"# �o�͗�\n" +
                $"- {instruction.sample}\n";
            string response = await _ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();

            // �o�͗�̃t�H�[�}�b�g�̉��߃~�X�Ő擪�� - ���t���Ă���ꍇ�B
            if (response[0] == '-')
            {
                response = response[1..];
            }

            // �o�͗�̃t�H�[�}�b�g�̉��߃~�X�Ŕ��p�X�y�[�X�������Ă���ꍇ�B
            // �䎌�Ƃ��Ĉ����̂Łu�v�t���ɂ��ďo�͂���ꍇ������B
            return response.Trim().Trim('�u', '�v');
#else
            // �f�o�b�O�p�B�䎌��AI�Ƀ��N�G�X�g�����A�T���v�������̂܂ܕԂ��B
            Debug.Log("API�ɑ䎌�����N�G�X�g���Ă��Ȃ���ԂŎ��s���B");
            return instruction.sample;
#endif
        }

        static (string, string) GetInstruction(RequestLineType type)
        {
            if (type == RequestLineType.Entry) return ("�o�ꎞ�̑䎌", "�撣�邼�I");
            if (type == RequestLineType.Defeated) return ("�G�Ƃ̃o�g���Ŕs�k�����ۂ̑䎌", "�������߂�");
            if (type == RequestLineType.Goal) return ("�Q�[�����N���A�����ۂ̑䎌", "�΂��΂�");
            if (type == RequestLineType.GetArtifactSuccess) return ("�`���̕󕨂���肵���ۂ̑䎌", "����́c�I");
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
