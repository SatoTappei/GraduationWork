using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        Dictionary<Adventurer, string> _adventureResults;

        void Awake()
        {
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
            gameManager.SetAdventureResult(adventurer, result);
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // 一度に生成する冒険者の最大数。
            const int Max = 4;

            AdventurerSpawner.TryFind(out AdventurerSpawner spawner);
            while (!token.IsCancellationRequested)
            {
                _adventureResults.Clear();

                // 一定間隔で冒険者を生成。
                int spawnedCount = await spawner.SpawnAsync(Max, token);

                // 生成した冒険者が全員、冒険の結果を報告するまで待つ。
                await UniTask.WaitUntil(() => _adventureResults.Count == spawnedCount, cancellationToken: token);

                // 冒険の結果を送信する。
                await AdventureResultSender.SendAsync(_adventureResults, token);
            }
        }

        void SetAdventureResult(Adventurer adventurer, string result)
        {
            _adventureResults.Add(adventurer, result);
        }
    }
}

// 冒険の結果を送信する際に死亡した場合は冒険者を補充するような処理を追加する。
// そのタイミングでのみ補充するので排他制御がいらなくなる？