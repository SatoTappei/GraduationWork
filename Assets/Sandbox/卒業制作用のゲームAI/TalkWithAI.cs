using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class TalkWithAI : MonoBehaviour
{
    [SerializeField] string _content;
    [SerializeField] string _problem;
    [SerializeField] string _hint1;
    [SerializeField] string _hint2;
    [SerializeField] string _hint3;

    GptRequest _memoryGpt;
    GptRequest _answerGpt;

    void Awake()
    {
        _memoryGpt = new GptRequest(
            $"# �w�����e\n" +
            $"- ���Ȃ��̓_���W�����ɐ���A�����T���Ă��܂��B\n" +
            $"- �^����ꂽ���͂������������q���g���ǂ����𔻒肵�Ă��������B\n" +
            $"'''\n" +
            $"# �o�͌`��\n" +
            $"- �q���g���Ɣ��肵���ꍇ 1\n" +
            $"- �q���g�ł͂Ȃ��Ɣ��肵���ꍇ 0\n"
            );

        _answerGpt = new GptRequest(
            $"# �w�����e\n" +
            $"- ���ƃq���g���^������̂ŁA�񓚂��Ă��������B\n" +
            $"- �q���g�ɂ͉R���������Ă���ꍇ������̂Ō��ɂ߂Ă��������B\n" +
            $"'''\n" +
            $"# ���͗�\n" +
            $"- ���u�ڂ̑O�̔���4���̌��̔ԍ��́H�v �q���g�u�Ȃ��v�u2042�v�u���̔ԍ���30�v\n" +
            $"# �o�͌`��\n" +
            $"- ��������ߒ��͏o�͂����A�񓚂����Ȍ��ɏo�͂��Ă��������B\n"
            );
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Talk(_content);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Solve();
    }

    void Talk(string content)
    {
        TalkAsync(content, this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTask TalkAsync(string content, CancellationToken token)
    {
        string response = await _memoryGpt.RequestAsync(content);

        if (token.IsCancellationRequested) return;

        if (IsHint(response))
        {
            Debug.Log(content + " �̓q���g�ł��B");
        }
        else
        {
            Debug.Log(content + " �̓q���g����Ȃ��ł��B");
        }
    }

    bool IsHint(string response)
    {
        Match m = Regex.Match(response, "[0-1]");
        if (m.Value == "1") return true;
        else if (m.Value == "0") return false;
        else return false;
    }

    void Solve()
    {
        SolveAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTask SolveAsync(CancellationToken token)
    {
        string s = $"���u{_problem}�v �q���g�u{_hint1}�v�u{_hint2}�v�u{_hint3}�v";
        string response = await _answerGpt.RequestAsync(s);

        if (token.IsCancellationRequested) return;

        Debug.Log($"������̓���: {response}");
    }
}
