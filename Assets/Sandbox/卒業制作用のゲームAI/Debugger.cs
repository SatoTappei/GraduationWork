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
        _root = new Node("Entrance", "入る場合は0を、入らない場合は1と答えてください。");
        Node a = new Node("1F_Hall", "左の扉に入る場合は0を、右の扉に入る場合は1と答えてください。");
        Node b = new Node("Kitchen", "裏口から出る場合は0を、このまま居座る場合は1と答えてください。");
        Node c = new Node("Stairs", "2階に上がる場合は0を、地下に降りる場合は1と答えてください。");
        Node d = new Node("2F_Hall", "左の扉に入る場合は0を、右の扉に入る場合は1と答えてください。");

        _root.AddChild(a);
        a.AddChild(b);
        a.AddChild(c);
        c.AddChild(d);

        _current = _root;
        _aiRequest = new GptRequest("あなたはある建物を訪れたサラリーマンです。建物を探索してください。");

        UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("キャラクターシート作る開始");
            CreateCharacterSheetAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
    }

    async UniTaskVoid UpdateAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Debug.Log($"今いるところ: {_current.ID}");

            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space), cancellationToken: token);
            try
            {
                //string response = await _aiRequest.RequestAsync(_current.Content);
                string response = await RequestAsyncDummy();
                Debug.Log("質問: " + _current.Content);
                Debug.Log("回答: " + response);

                if (_current.IsLeaf()) break;

                int index = ToIndex(response);
                if (index != -1)
                {
                    _current = _current.GetChild(ToIndex(response));
                }
            }
            catch(UnityWebRequestException _)
            {
                Debug.LogError("例外");
            }
        }

        Debug.Log("おわり: " + _current.ID);
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
            "TRPGのキャラクターシートを作成します。以下はキャラクターの説明です。" +
            "性格が悪く、人の悪口をよく言う。しかし、仕事は納期ギリギリにキチンとこなす。" +
            "尖った思想を持っているため取扱注意。タバコが好きで四六時中吸っている。" +
            "ライトノベルを読み、部活もサボり気味だったようで、運動は苦手そうだ。" +
            "上記の設定を基に次から質問します。"
            );

        string str = await gptRequest.RequestAsync(Content("体力"));
        Debug.Log("体力: " + str);
        if (token.IsCancellationRequested) return;

        string inte = await gptRequest.RequestAsync(Content("知力"));
        Debug.Log("知力: " + inte);
        if (token.IsCancellationRequested) return;

        string charm = await gptRequest.RequestAsync(Content("魅力"));
        Debug.Log("魅力: " + charm);
        if (token.IsCancellationRequested) return;

        string dex = await gptRequest.RequestAsync(Content("器用さ"));
        Debug.Log("器用さ: " + dex);
        if (token.IsCancellationRequested) return;

        string sens = await gptRequest.RequestAsync(Content("感受性"));
        Debug.Log("感受性: " + sens);
        if (token.IsCancellationRequested) return;

        Debug.Log("キャラクターシート作る終わり");

        string Content(string paramName)
        {
            return $"説明したキャラクターの設定を基に、{paramName}のパラメータを1から100の値で設定します。1から100の値のみを答えてください。";
        }
    }
}