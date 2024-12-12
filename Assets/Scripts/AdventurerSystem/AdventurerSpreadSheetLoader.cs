using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public static class AdventurerSpreadSheetLoader
    {
        public static async UniTask<IReadOnlyList<AdventurerSpreadSheetData>> GetDataAsync(CancellationToken token)
        {
            const string FileID = "1hQSCky3xafLS3p75MMYjUovEfKLtOvIh9U4HdDNUElk";
            const string SheetName = "プロフィール";

            string URL = $"https://docs.google.com/spreadsheets/d/{FileID}/gviz/tq?tqx=out:csv&sheet={SheetName}";
            using UnityWebRequest request = UnityWebRequest.Get(URL);

            try
            {
                await request.SendWebRequest().WithCancellation(token);
            }
            catch(UnityWebRequestException e)
            {
                Debug.LogError(e);
                return new List<AdventurerSpreadSheetData>();
            }

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(request.result);
                return new List<AdventurerSpreadSheetData>();
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.result);
                return new List<AdventurerSpreadSheetData>();
            }
            else
            {
                return Parse(request.downloadHandler.text);
            }
        }

        static IReadOnlyList<AdventurerSpreadSheetData> Parse(string downloadText)
        {
            List<AdventurerSpreadSheetData> profiles = new List<AdventurerSpreadSheetData>();

            // 1行目はラベルなのでスキップ。
            foreach (string row in downloadText.Split("\n").Skip(1))
            {
                string[] cells = row.Split(",").Select(c => c.Trim('"')).ToArray();
                AdventurerSpreadSheetData profile = new AdventurerSpreadSheetData(
                    cells[0],  // 名前
                    cells[1],  // 表示名
                    cells[2],  // 性別
                    cells[3],  // 年齢
                    cells[4],  // 職業
                    cells[5],  // 性格
                    cells[6],  // モチベーション
                    cells[7],  // 弱点
                    cells[8]); // バックストーリー

                if (profile.IsFieldEmpty()) continue;

                profiles.Add(profile);
            }

            return profiles;
        }
    }
}
