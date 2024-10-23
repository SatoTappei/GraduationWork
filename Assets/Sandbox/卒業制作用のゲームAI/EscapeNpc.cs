//#define デバッグ

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class SearchPoint
{
    [SerializeField] string _name;
    [SerializeField] Transform _point;
    [SerializeField] Vector3 _rotation;

    public string Name => _name;
    public Vector3 Position => _point.position;
    public Quaternion Rotation => Quaternion.Euler(_rotation);
}

public class EscapeNpc : MonoBehaviour
{
    [SerializeField] SearchPoint[] _searchPoints;

    GptRequest _gpt;
    Queue<string> _memory;

    bool _isDoorOpen;

    void Awake()
    {
        _gpt = new GptRequest(
            $"# 指示内容\n" +
            $"- あなたは密室に閉じ込められています。\n" +
            $"- 謎解きとヒントが与えられるので、脱出するために謎を解いてください。\n"
            );

        _memory = new Queue<string>();
    }

    void Start()
    {
        UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTask UpdateAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            int search = await SearchAsync(token);
            int action = await ActionAsync(search, token);
            string item = await ChooseItemAsync(search, action, token);

            AddMemory(search, action, item);
            Flag(search, action, item);

            if (IsGameClear(search, action, item)) break;

            await UniTask.WaitForSeconds(1.0f);
        }

        Debug.Log("ゲームクリア！");
    }

    async UniTask<int> SearchAsync(CancellationToken token)
    {
        string s =
            "# 指示内容\n" +
            "- 選択肢の中から脱出の手掛かりになりそうな行動を選んでください。\n" +
            "'''\n";

        string[] choices = GetChoices();

        string c = "# 選択肢\n";
        for (int i = 0; i < choices.Length; i++)
        {
            c += $"- {choices[i]}\n";
        }
        c += "'''\n";

        string t = "# 出力形式\n";
        for (int i = 0; i < choices.Length; i++)
        {
            t += $"- {choices[i]}場合は{i}と答えてください。\n";
        }

#if デバッグ
        t += "- いずれの選択肢の場合でも、その選択をした理由も合わせて答えてください。";
#endif

        string response = await _gpt.RequestAsync(s + GetHint() + c + t);

        if (token.IsCancellationRequested) return -1;

        Debug.Log($"どこを調べるかに対する回答: " + response);

        if (TryParse(response, out int result))
        {
            transform.position = _searchPoints[result].Position;
            transform.rotation = _searchPoints[result].Rotation;

            return result;
        }
        else return -1;
    }

    string GetHint()
    {
        string hint = "# ヒント\n";
        foreach (string s in _memory)
        {
            hint += $"- {s}\n";
        }
        hint += "'''\n";

        return hint;
    }

    bool TryParse(string response, out int result)
    {
        Match m = Regex.Match(response, "[0-9]");
        if (int.TryParse(m.Value, out result)) return true;
        else return false;
    }

    string[] GetChoices()
    {
        string[] choices =
        {
            "右側のドアを調べる",
            "奥のドアを調べる",
            "机を調べる",
            "床に散らばったコインを調べる",
            "コンテナを調べる",
            "祭壇を調べる"
        };

        return choices;
    }

    async UniTask<int> ActionAsync(int search, CancellationToken token)
    {
        string[] choices = GetChoices();
        string s =
            $"# 指示内容\n" +
            $"- あなたは{choices[search]}ことにしました。\n" +
            $"- 選択肢の中から調べる方法を選んでください。\n" +
            $"'''\n";

        string[] actions = GetActions();
        string c = "# 選択肢\n";
        for (int i = 0; i < choices.Length; i++)
        {
            c += $"- {actions[i]}\n";
        }
        c += "'''\n";

        string t = "# 出力形式\n";
        for (int i = 0; i < choices.Length; i++)
        {
            t += $"- {actions[i]}場合は{i}と答えてください。\n";
        }

#if デバッグ
        t += "- いずれの選択肢の場合でも、その選択をした理由も合わせて答えてください。";
#endif

        string response = await _gpt.RequestAsync(s + GetHint() + c + t);

        if (token.IsCancellationRequested) return -1;

        Debug.Log($"どうやって調べるかに対する回答: " + response);

        if (TryParse(response, out int result))
        {
            return result;
        }
        else return -1;
    }

    string[] GetActions()
    {
        string[] actions =
        {
            "引く",
            "押す",
            "話しかける",
            "叫ぶ",
            "掃除する",
            "踊る",
            "舐める"
        };

        return actions;
    }

    async UniTask<string> ChooseItemAsync(int search, int action, CancellationToken token)
    {
        string[] choices = GetChoices();
        string[] actions = GetActions();
        string[] items = GetItems();
        string s =
            $"# 指示内容\n" +
            $"- あなたは{choices[search]}方法として、{actions[action]}ことにしました。\n" +
            $"- {items[action]}\n" +
            $"'''\n";

        string t =
        "# 出力形式\n" +
#if デバッグ
        "- いずれの選択肢の場合でも、その選択をした理由も合わせて答えてください。";
#else
        "- 回答のみを簡潔に出力してください。";
#endif

        string response = await _gpt.RequestAsync(s + GetHint() + t);

        if (token.IsCancellationRequested) return string.Empty;

        response = response.Trim('「', '」');

        Debug.Log("何を使うかに対する回答: " + response);

        return response;
    }

    string[] GetItems()
    {
        string[] items =
        {
            "使うアイテムを回答してください。",
            "使うアイテムを回答してください。",
            "話しかける言葉を回答してください。",
            "叫ぶ言葉を回答してください。",
            "使うアイテムを回答してください。",
            "踊る曲を回答してください。",
            "正気ですか？"
        };

        return items;
    }

    void AddMemory(int search, int action, string item)
    {
        if (_memory.Count > 5) _memory.Dequeue();

        if (search == 5 && action == 3 && item == "ゆんやぁ！")
        {
            _memory.Enqueue("カチッと音がしてドアが開いた。");
        }
        else if (search == 0)
        {
            _memory.Enqueue("右側のドアは開かず、特に仕掛けもない。");
        }
        else if (search == 1)
        {
            _memory.Enqueue("奥のドアは何らかの仕掛けで開くようだ。");
        }
        else if (search == 2)
        {
            _memory.Enqueue("「祭壇で叫ぶ」というメモを見つけた。");
        }
        else if (search == 3)
        {
            _memory.Enqueue("「ゆんやぁ！」という言葉が書かれたメモを見つけた。");
        }
        else if (search == 4)
        {
            _memory.Enqueue("コンテナには何もない。");
        }
        else if (search == 5)
        {
            _memory.Enqueue("祭壇には謎の仕掛けがある。");
        }
    }

    void Flag(int search, int action, string item)
    {
        if(search == 5 && action == 3 && item == "ゆんやぁ！")
        {
            _isDoorOpen = true;
        }
    }

    bool IsGameClear(int search, int action, string item)
    {
        return _isDoorOpen && search == 1;
    }
}