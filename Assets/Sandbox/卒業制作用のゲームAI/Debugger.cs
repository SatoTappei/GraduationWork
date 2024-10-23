using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;

public class Debugger : MonoBehaviour
{
    [SerializeField] CharacterSheet _sheet;

    GptRequest _aiRequest;
    Node _root;
    Node _current;

    void Start()
    {
        _root = new Node("Entrance", "����ꍇ��0���A����Ȃ��ꍇ��1�Ɠ����Ă��������B");
        Node a = new Node("1F_Hall", "���̔��ɓ���ꍇ��0���A�E�̔��ɓ���ꍇ��1�Ɠ����Ă��������B");
        Node b = new Node("Kitchen", "��������o��ꍇ��0���A���̂܂܋�����ꍇ��1�Ɠ����Ă��������B");
        Node c = new Node("Stairs", "2�K�ɏオ��ꍇ��0���A�n���ɍ~���ꍇ��1�Ɠ����Ă��������B");
        Node d = new Node("2F_Hall", "���̔��ɓ���ꍇ��0���A�E�̔��ɓ���ꍇ��1�Ɠ����Ă��������B");

        _root.AddChild(a);
        a.AddChild(b);
        a.AddChild(c);
        c.AddChild(d);

        _current = _root;
        _aiRequest = new GptRequest("���Ȃ��͂��錚����K�ꂽ�T�����[�}���ł��B������T�����Ă��������B");

        UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("�L�����N�^�[�V�[�g���J�n");
            CreateCharacterSheetAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
    }

    async UniTaskVoid UpdateAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Debug.Log($"������Ƃ���: {_current.ID}");

            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space), cancellationToken: token);
            try
            {
                //string response = await _aiRequest.RequestAsync(_current.Content);
                string response = await RequestAsyncDummy();
                Debug.Log("����: " + _current.Content);
                Debug.Log("��: " + response);

                if (_current.IsLeaf()) break;

                int index = ToIndex(response);
                if (index != -1)
                {
                    _current = _current.GetChild(ToIndex(response));
                }
            }
            catch(UnityWebRequestException _)
            {
                Debug.LogError("��O");
            }
        }

        Debug.Log("�����: " + _current.ID);
    }

    async UniTask<string> RequestAsyncDummy()
    {
        await UniTask.Yield();
        return Random.Range(0, 2).ToString();
    }

    int ToIndex(string response)
    {
        Match m = Regex.Match(response, "[0-9]");
        if (int.TryParse(m.Value, out int result)) return result;
        else return -1;
    }

    async UniTask CreateCharacterSheetAsync(CancellationToken token)
    {
        GptRequest gptRequest = new GptRequest(
            "TRPG�̃L�����N�^�[�V�[�g���쐬���܂��B�ȉ��̓L�����N�^�[�̐����ł��B" +
            "���i�������A�l�̈������悭�����B�������A�d���͔[���M���M���ɃL�`���Ƃ��Ȃ��B" +
            "������v�z�������Ă��邽�ߎ戵���ӁB�^�o�R���D���Ŏl�Z�����z���Ă���B" +
            "���C�g�m�x����ǂ݁A�������T�{��C���������悤�ŁA�^���͋�肻�����B" +
            "��L�̐ݒ����Ɏ����玿�₵�܂��B"
            );

        string str = await gptRequest.RequestAsync(Content("�̗�"));
        Debug.Log("�̗�: " + str);
        if (token.IsCancellationRequested) return;

        string inte = await gptRequest.RequestAsync(Content("�m��"));
        Debug.Log("�m��: " + inte);
        if (token.IsCancellationRequested) return;

        string charm = await gptRequest.RequestAsync(Content("����"));
        Debug.Log("����: " + charm);
        if (token.IsCancellationRequested) return;

        string dex = await gptRequest.RequestAsync(Content("��p��"));
        Debug.Log("��p��: " + dex);
        if (token.IsCancellationRequested) return;

        string sens = await gptRequest.RequestAsync(Content("����"));
        Debug.Log("����: " + sens);
        if (token.IsCancellationRequested) return;

        Debug.Log("�L�����N�^�[�V�[�g���I���");

        string Content(string paramName)
        {
            return $"���������L�����N�^�[�̐ݒ����ɁA{paramName}�̃p�����[�^��1����100�̒l�Őݒ肵�܂��B1����100�̒l�݂̂𓚂��Ă��������B";
        }
    }
}