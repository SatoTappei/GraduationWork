using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public enum RequestLineType
    {
        None,
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

    // Awake�̃^�C�~���O�ō��̒l���Q�Ƃ���̂ŁA���̏��������������悱�̃X�N���v�g��AddComponent����B
    public class RolePlayAI : MonoBehaviour
    {
        Blackboard _blackboard;
        AIRequest _ai;

        void Awake()
        {
            // �L�����N�^�[�Ƃ��ĐU�镑��AI�͑䎌��w�i�Ȃǂ�UI�ɕ\������̂œ��{��B
            _blackboard = GetComponent<Blackboard>();
            string age = _blackboard.AdventurerSheet.Age;
            string job = _blackboard.AdventurerSheet.Job;
            string background = _blackboard.AdventurerSheet.Background;
            string prompt =
                $"# �w�����e\n" +
                $"- �ȉ��̃L�����N�^�[�ɂȂ肫���Ċe����ɓ����Ă��������B\n" +
                $"'''\n" +
                $"# �L�����N�^�[\n" +
                $"- {age}�΂�{job}�B\n" +
                $"- {background}\n";

            _ai = AIRequestFactory.Create(prompt);
        }

        public async UniTask<string> RequestLineAsync(RequestLineType type, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            (string content, string sample) instruction = GetContentAndSample(type);

            string prompt =
                $"# �w�����e\n" +
                $"- ���g�̃L�����N�^�[�̐ݒ����ɁA{instruction.content}���l���Ă��������B\n" +
                $"- �Z���ꌾ�ő䎌�����肢���܂��B\n" +
                $"- {GetLineEmotion()}\n" +
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

        string GetLineEmotion()
        {
            // �S��̒l�ɂ���đ䎌�̊���ω�����B臒l�͓K���ɐݒ�B
            if (_blackboard.CurrentEmotion < _blackboard.MaxEmotion / 10 * 7)
            {
                return "���i��菭���e���V�������߂ł��肢���܂��B";
            }
            else if (_blackboard.CurrentEmotion < _blackboard.MaxEmotion / 10 * 3)
            {
                return "���i���e���V������߂ł��肢���܂��B";
            }
            else
            {
                return "���i�̃e���V�����ł��肢���܂��B";
            }
        }
    }
}
