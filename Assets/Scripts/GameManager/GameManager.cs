using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VTNConnect;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        AdventurerSpawner _adventurerSpawner;
        Dictionary<Adventurer, string> _adventureResults;

        void Awake()
        {
            _adventurerSpawner = AdventurerSpawner.Find();
            _adventureResults = new Dictionary<Adventurer, string>();
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        // 冒険者が脱出や撃破された場合、冒険者側から呼び出して冒険の結果を報告する。
        public static void ReportAdventureResult(Adventurer adventurer, string result)
        {
            GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager._adventureResults.Add(adventurer, result);
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // 一度に生成する冒険者の最大数。
            const int Max = 1;

            while (!token.IsCancellationRequested)
            {
                // ゲーム開始。
                await VantanConnect.GameStart();
                token.ThrowIfCancellationRequested();

                _adventureResults.Clear();

                // 一定間隔で冒険者を生成。
                int spawnedCount = await _adventurerSpawner.SpawnAsync(Max, token);

                // 生成した冒険者が全員、冒険の結果を報告するまで待つ。
                await UniTask.WaitUntil(() => _adventureResults.Count == spawnedCount, cancellationToken: token);

                // ゲーム終了。
                await VantanConnect.GameEnd();
                token.ThrowIfCancellationRequested();
            }
        }
    }
}