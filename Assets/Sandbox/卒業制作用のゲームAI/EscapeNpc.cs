//#define �f�o�b�O

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
            $"# �w�����e\n" +
            $"- ���Ȃ��͖����ɕ����߂��Ă��܂��B\n" +
            $"- ������ƃq���g���^������̂ŁA�E�o���邽�߂ɓ�������Ă��������B\n"
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

        Debug.Log("�Q�[���N���A�I");
    }

    async UniTask<int> SearchAsync(CancellationToken token)
    {
        string s =
            "# �w�����e\n" +
            "- �I�����̒�����E�o�̎�|����ɂȂ肻���ȍs����I��ł��������B\n" +
            "'''\n";

        string[] choices = GetChoices();

        string c = "# �I����\n";
        for (int i = 0; i < choices.Length; i++)
        {
            c += $"- {choices[i]}\n";
        }
        c += "'''\n";

        string t = "# �o�͌`��\n";
        for (int i = 0; i < choices.Length; i++)
        {
            t += $"- {choices[i]}�ꍇ��{i}�Ɠ����Ă��������B\n";
        }

#if �f�o�b�O
        t += "- ������̑I�����̏ꍇ�ł��A���̑I�����������R�����킹�ē����Ă��������B";
#endif

        string response = await _gpt.RequestAsync(s + GetHint() + c + t);

        if (token.IsCancellationRequested) return -1;

        Debug.Log($"�ǂ��𒲂ׂ邩�ɑ΂����: " + response);

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
        string hint = "# �q���g\n";
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
            "�E���̃h�A�𒲂ׂ�",
            "���̃h�A�𒲂ׂ�",
            "���𒲂ׂ�",
            "���ɎU��΂����R�C���𒲂ׂ�",
            "�R���e�i�𒲂ׂ�",
            "�Ւd�𒲂ׂ�"
        };

        return choices;
    }

    async UniTask<int> ActionAsync(int search, CancellationToken token)
    {
        string[] choices = GetChoices();
        string s =
            $"# �w�����e\n" +
            $"- ���Ȃ���{choices[search]}���Ƃɂ��܂����B\n" +
            $"- �I�����̒����璲�ׂ���@��I��ł��������B\n" +
            $"'''\n";

        string[] actions = GetActions();
        string c = "# �I����\n";
        for (int i = 0; i < choices.Length; i++)
        {
            c += $"- {actions[i]}\n";
        }
        c += "'''\n";

        string t = "# �o�͌`��\n";
        for (int i = 0; i < choices.Length; i++)
        {
            t += $"- {actions[i]}�ꍇ��{i}�Ɠ����Ă��������B\n";
        }

#if �f�o�b�O
        t += "- ������̑I�����̏ꍇ�ł��A���̑I�����������R�����킹�ē����Ă��������B";
#endif

        string response = await _gpt.RequestAsync(s + GetHint() + c + t);

        if (token.IsCancellationRequested) return -1;

        Debug.Log($"�ǂ�����Ē��ׂ邩�ɑ΂����: " + response);

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
            "����",
            "����",
            "�b��������",
            "����",
            "�|������",
            "�x��",
            "�r�߂�"
        };

        return actions;
    }

    async UniTask<string> ChooseItemAsync(int search, int action, CancellationToken token)
    {
        string[] choices = GetChoices();
        string[] actions = GetActions();
        string[] items = GetItems();
        string s =
            $"# �w�����e\n" +
            $"- ���Ȃ���{choices[search]}���@�Ƃ��āA{actions[action]}���Ƃɂ��܂����B\n" +
            $"- {items[action]}\n" +
            $"'''\n";

        string t =
        "# �o�͌`��\n" +
#if �f�o�b�O
        "- ������̑I�����̏ꍇ�ł��A���̑I�����������R�����킹�ē����Ă��������B";
#else
        "- �񓚂݂̂��Ȍ��ɏo�͂��Ă��������B";
#endif

        string response = await _gpt.RequestAsync(s + GetHint() + t);

        if (token.IsCancellationRequested) return string.Empty;

        response = response.Trim('�u', '�v');

        Debug.Log("�����g�����ɑ΂����: " + response);

        return response;
    }

    string[] GetItems()
    {
        string[] items =
        {
            "�g���A�C�e�����񓚂��Ă��������B",
            "�g���A�C�e�����񓚂��Ă��������B",
            "�b�������錾�t���񓚂��Ă��������B",
            "���Ԍ��t���񓚂��Ă��������B",
            "�g���A�C�e�����񓚂��Ă��������B",
            "�x��Ȃ��񓚂��Ă��������B",
            "���C�ł����H"
        };

        return items;
    }

    void AddMemory(int search, int action, string item)
    {
        if (_memory.Count > 5) _memory.Dequeue();

        if (search == 5 && action == 3 && item == "���₟�I")
        {
            _memory.Enqueue("�J�`�b�Ɖ������ăh�A���J�����B");
        }
        else if (search == 0)
        {
            _memory.Enqueue("�E���̃h�A�͊J�����A���Ɏd�|�����Ȃ��B");
        }
        else if (search == 1)
        {
            _memory.Enqueue("���̃h�A�͉��炩�̎d�|���ŊJ���悤���B");
        }
        else if (search == 2)
        {
            _memory.Enqueue("�u�Ւd�ŋ��ԁv�Ƃ����������������B");
        }
        else if (search == 3)
        {
            _memory.Enqueue("�u���₟�I�v�Ƃ������t�������ꂽ�������������B");
        }
        else if (search == 4)
        {
            _memory.Enqueue("�R���e�i�ɂ͉����Ȃ��B");
        }
        else if (search == 5)
        {
            _memory.Enqueue("�Ւd�ɂ͓�̎d�|��������B");
        }
    }

    void Flag(int search, int action, string item)
    {
        if(search == 5 && action == 3 && item == "���₟�I")
        {
            _isDoorOpen = true;
        }
    }

    bool IsGameClear(int search, int action, string item)
    {
        return _isDoorOpen && search == 1;
    }
}