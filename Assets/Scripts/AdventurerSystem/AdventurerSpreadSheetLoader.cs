using Cysharp.Threading.Tasks;
using System.Collections;
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
            const string SheetName = "シート1";

            string URL = $"https://docs.google.com/spreadsheets/d/{FileID}/gviz/tq?tqx=out:csv&sheet={SheetName}";
            using UnityWebRequest request = UnityWebRequest.Get(URL);
            await request.SendWebRequest();

            token.ThrowIfCancellationRequested();

            if (IsSuccess(request.result))
            {
                return Parse(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("スプレッドシートから冒険者のプロフィールを読み込めなかった。");

                return null;
            }
        }

        static bool IsSuccess(UnityWebRequest.Result result)
        {
            if (result == UnityWebRequest.Result.ConnectionError) return false;
            if (result == UnityWebRequest.Result.ProtocolError) return false;
            else return true;
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
