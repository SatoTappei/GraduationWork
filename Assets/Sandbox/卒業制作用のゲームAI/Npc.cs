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
            $"# �w�����e\n" +
            $"- �L�����N�^�[�Ƃ��ĐU�����Ă��������B\n" +
            $"'''\n" +
            $"# ����\n" + 
            $"- {sheet.Age}�΂�{sheet.Job}�B\n" +
            $"- {sheet.Background}\n" +
            $"'''\n" +
            $"# �o�͌`��\n" +
            $"- 10�������x�̊Ȍ��ȕ�����B\n" +
            $"'''\n" +
            $"# �o�͗�\n" +
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
            $"# �w�����e\n" +
            $"- �ȉ��̕��͂ɑ΂��āA���R�ȉ�b������ꍇ�A�Ԏ�������K�v�����邩�`�F�b�N���Ă��������B\n" +
            $"'''\n" +
            $"# ����\n" +
            $"- {content}\n" +
            $"'''\n" +
            $"# �K�v�Ȃ���\n" +
            $"- �J���[����B\n" +
            $"- ����B\n" +
            $"- �������ˁB\n" +
            $"'''\n" +
            $"# �o�͌`��\n" +
            $"- �K�v����ꍇ�� 1\n" +
            $"- �K�v�Ȃ��ꍇ�� 0";

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
            $"�ȉ��̕��͂ւ̕Ԏ��Ƃ��ēK�؂��Ȍ��ȕ��͂��l���Ă��������B\n" +
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
