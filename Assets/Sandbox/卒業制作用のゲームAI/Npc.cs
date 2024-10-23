using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Npc : MonoBehaviour
{
    [SerializeField] CharacterSheetTemplate _sheetTemplate;
    [SerializeField] Text _line;

    GptRequest _gpt;
    Npc[] _npcs;
    Coroutine _lineAnimation;

    void Awake()
    {
        CharacterSheet sheet = _sheetTemplate.Value;
        _gpt = new GptRequest(
            $"# 指示内容\n" +
            $"- キャラクターとして振舞ってください。\n" +
            $"'''\n" +
            $"# 役割\n" + 
            $"- {sheet.Age}歳の{sheet.Job}。\n" +
            $"- {sheet.Background}\n" +
            $"'''\n" +
            $"# 出力形式\n" +
            $"- 10文字程度の簡潔な文字列。\n" +
            $"'''\n" +
            $"# 出力例\n" +
            $"- {sheet.LineSample1}\n" +
            $"- {sheet.LineSample2}\n" +
            $"- {sheet.LineSample3}"
            );

        _npcs = FindObjectsByType<Npc>(FindObjectsSortMode.None).Where(n => n != this).ToArray();
    }

    void Start()
    {
        PlayLineAnimation(string.Empty, 0);
    }

    public void Talk(string content)
    {
        TalkAsync(content, this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTask TalkAsync(string content, CancellationToken token)
    {
        if (await IsTalkEndAsync(content, token)) return;

        string response = await _gpt.RequestAsync(CreateRequestContent(content));

        if (token.IsCancellationRequested) return;

        PlayLineAnimation(response, 2.0f);
        await IntervalAsync(2.0f).ToUniTask(this);
        GetRandomTarget().Talk(response);
    }

    async UniTask<bool> IsTalkEndAsync(string content, CancellationToken token)
    {
        return !await IsTalkContinueAsync(content, token);
    }

    async UniTask<bool> IsTalkContinueAsync(string content, CancellationToken token)
    {
        string s = 
            $"# 指示内容\n" +
            $"- 以下の文章に対して、自然な会話をする場合、返事をする必要があるかチェックしてください。\n" +
            $"'''\n" +
            $"# 文章\n" +
            $"- {content}\n" +
            $"'''\n" +
            $"# 必要ない例\n" +
            $"- カレーだよ。\n" +
            $"- うん。\n" +
            $"- そうだね。\n" +
            $"'''\n" +
            $"# 出力形式\n" +
            $"- 必要ある場合は 1\n" +
            $"- 必要ない場合は 0";

        string response = await _gpt.RequestAsync(s);

        if (token.IsCancellationRequested) return false;

        return IsTalkContinueResponse(response);
    }

    bool IsTalkContinueResponse(string response)
    {
        Match m = Regex.Match(response, "[0-1]");
        if (m.Value == "1") return true;
        else if (m.Value == "0") return false;
        else return false;
    }

    string CreateRequestContent(string content)
    {
        string s =
            $"以下の文章への返事として適切かつ簡潔な文章を考えてください。\n" +
            $"'''\n" +
            $"{content}\n";

        return s;
    }

    void PlayLineAnimation(string content, float lifeTime)
    {
        if (_lineAnimation != null) StopCoroutine(_lineAnimation);
        _lineAnimation = StartCoroutine(PlayLineAnimationAsync(content, lifeTime));
    }

    IEnumerator PlayLineAnimationAsync(string content, float lifeTime)
    {
        _line.text = content;
        yield return new WaitForSeconds(lifeTime);
        _line.text = string.Empty;
    }

    IEnumerator IntervalAsync(float time)
    {
        yield return new WaitForSeconds(time);
    }

    Npc GetRandomTarget()
    {
        int i = Random.Range(0, _npcs.Length);
        return _npcs[i];
    }
}
