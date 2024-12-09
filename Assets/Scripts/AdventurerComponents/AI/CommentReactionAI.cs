using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using AI;

namespace Game
{
    public class CommentReactionAI : MonoBehaviour
    {
        public class Response
        {
            public string Line;
            public float Score;
        }

        public async UniTask<Response> RequestReactionAsync(IReadOnlyCollection<CommentSpreadSheetData> comment, CancellationToken token)
        {
            (string line, float score) = await (RequestLineAsync(comment, token), EvaluateAsync(comment, token));
            return new Response { Line = line, Score = score };
        }

        async UniTask<string> RequestLineAsync(IReadOnlyCollection<CommentSpreadSheetData> comment, CancellationToken token)
        {
            AIClient ai = CreateAI();
            string prompt =
                $"# �w�����e\n" +
                $"- ���g�̃L�����N�^�[�̐ݒ����ɁA���̑䎌�ɑ΂���ԓ����l���Ă��������B\n" +
                $"- �u{GetCommentText(comment)}�v\n" +
                $"- �Z���ꌾ�ő䎌�����肢���܂��B\n" +
                $"'''\n" +
                $"# �o�͌`��\n" +
                $"- �䎌�݂̂����̂܂܏o�͂��Ă��������B\n" +
                $"'''\n" +
                $"# �o�͗�\n" +
                $"- �������肪�Ƃ��B\n" +
                $"- �߂����Ɋ撣�邼�I\n";
            string response = await ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();

            if (response[0] == '-') response = response[1..];

            return response.Trim().Trim('�u', '�v');
        }

        async UniTask<float> EvaluateAsync(IReadOnlyCollection<CommentSpreadSheetData> comment, CancellationToken token)
        {
            AIClient ai = CreateAI();
            string prompt =
                $"# �w�����e\n" +
                $"- ���g�̃L�����N�^�[�̐ݒ����ɁA�䎌�u{GetCommentText(comment)}�v�ɑ΂��A�ǂ̂悤�Ȉ�ۂ����������Ă��������B\n" +
                $"- �|�W�e�B�u�Ȉ�ۂ��l�K�e�B�u�Ȉ�ہA�ǂ���ł����H\n" +
                $"'''\n" +
                $"# �o�͌`��\n" +
                $"- �|�W�e�B�u�Ȉ�ۂ̏ꍇ�́A�x�������傫���ق�1�ɋ߂����l���A�ア�ꍇ��0�ɋ߂����l���o�͂��Ă��������B\n" +
                $"- �l�K�e�B�u�Ȉ�ۂ̏ꍇ�́A�x�������傫���ق�-1�ɋ߂����l���A�ア�ꍇ��0�ɋ߂����l���o�͂��Ă��������B\n" +
                $"'''\n" +
                $"# �o�͗�\n" +
                $"- 1\n" +
                $"- -0.2\n";
            string response = await ai.RequestAsync(prompt, token);
            token.ThrowIfCancellationRequested();

            if (response[0] == '-') response = response[1..];

            if (float.TryParse(response, out float result)) return result;
            else
            {
                Debug.LogWarning($"{nameof(CommentReactionAI)}�̏o�͌`�����������Ȃ��B{response}");
                return 0;
            }
        }

        string GetCommentText(IReadOnlyCollection<CommentSpreadSheetData> comment)
        {
            // �����_���őI�����邱�ƂŁA�����R�����g�������ꍇ�̓|�W�e�B�u�ɁA
            // �\�����排����R�����g�������ꍇ�̓l�K�e�B�u�ɂȂ�₷���B
            int r = Random.Range(0, comment.Count);
            return comment.ElementAt(r).Comment;
        }

        AIClient CreateAI()
        {
            Blackboard blackboard = GetComponent<Blackboard>();
            string age = blackboard.AdventurerSheet.Age;
            string job = blackboard.AdventurerSheet.Job;
            string background = blackboard.AdventurerSheet.Background;
            string prompt =
                $"# �w�����e\n" +
                $"- �ȉ��̃L�����N�^�[�ɂȂ肫���Ċe����ɓ����Ă��������B\n" +
                $"'''\n" +
                $"# �L�����N�^�[\n" +
                $"- {age}�΂�{job}�B\n" +
                $"- {background}\n";

            return new AIClient(prompt);
        }
    }
}
