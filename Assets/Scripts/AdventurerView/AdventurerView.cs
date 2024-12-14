using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class AdventurerView : MonoBehaviour
    {
        [SerializeField] AdventurerViewUI[] _ui;

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                IEnumerable<IReadOnlyList<string>> data = await GetDataAsync(token);
                IReadOnlyList<IReadOnlyList<string>> profiles = data.Take(_ui.Length).ToList();
                for (int i = 0; i < profiles.Count; i++)
                {
                    _ui[i].SetProfile(
                        profiles[i][0],  // ���O
                        profiles[i][2],  // ����
                        profiles[i][3],  // �N��
                        profiles[i][4],  // �E��
                        profiles[i][5],  // ���i
                        profiles[i][6],  // ���`�x�[�V����
                        profiles[i][7],  // ��_
                        profiles[i][8]); // �o�b�N�X�g�[���[
                }

                await UniTask.Yield(cancellationToken: token);
            }
        }

        async UniTask<IEnumerable<IReadOnlyList<string>>> GetDataAsync(CancellationToken token)
        {
            const string FileID = "1hQSCky3xafLS3p75MMYjUovEfKLtOvIh9U4HdDNUElk";
            const string SheetName = "�g�p��";

            string URL = $"https://docs.google.com/spreadsheets/d/{FileID}/gviz/tq?tqx=out:csv&sheet={SheetName}";
            using UnityWebRequest request = UnityWebRequest.Get(URL);
            await request.SendWebRequest().WithCancellation(token);

            if (IsSuccess(request.result))
            {
                return Parse(request.downloadHandler.text);
            }
            else
            {
                return new List<List<string>>();
            }
        }

        static bool IsSuccess(UnityWebRequest.Result result)
        {
            if (result == UnityWebRequest.Result.ConnectionError) return false;
            if (result == UnityWebRequest.Result.ProtocolError) return false;
            else return true;
        }

        static IEnumerable<IReadOnlyList<string>> Parse(string downloadText)
        {
            // 1�s�ڂ̓��x���Ȃ̂ŃX�L�b�v�B
            foreach (string row in downloadText.Split("\n").Skip(1))
            {
                yield return row.Split(",").Select(c => c.Trim('"')).ToArray();
            }
        }
    }
}
