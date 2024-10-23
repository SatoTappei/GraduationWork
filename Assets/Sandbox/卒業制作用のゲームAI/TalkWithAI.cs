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
            $"# 指示内容\n" +
            $"- あなたはダンジョンに潜り、お宝を探しています。\n" +
            $"- 与えられた文章がお宝を見つけるヒントかどうかを判定してください。\n" +
            $"'''\n" +
            $"# 出力形式\n" +
            $"- ヒントだと判定した場合 1\n" +
            $"- ヒントではないと判定した場合 0\n"
            );

        _answerGpt = new GptRequest(
            $"# 指示内容\n" +
            $"- 問題とヒントが与えられるので、回答してください。\n" +
            $"- ヒントには嘘が混じっている場合があるので見極めてください。\n" +
            $"'''\n" +
            $"# 入力例\n" +
            $"- 問題「目の前の扉の4桁の鍵の番号は？」 ヒント「なし」「2042」「鍵の番号は30」\n" +
            $"# 出力形式\n" +
            $"- 謎を解く過程は出力せず、回答だけ簡潔に出力してください。\n"
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
            Debug.Log(content + " はヒントです。");
        }
        else
        {
            Debug.Log(content + " はヒントじゃないです。");
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
        string s = $"問題「{_problem}」 ヒント「{_hint1}」「{_hint2}」「{_hint3}」";
        string response = await _answerGpt.RequestAsync(s);

        if (token.IsCancellationRequested) return;

        Debug.Log($"謎解きの答え: {response}");
    }
}
