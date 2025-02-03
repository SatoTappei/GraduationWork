using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VTNConnect;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        AdventurerSpawner _spawner;
        int _resultCount;

        void Awake()
        {
            _spawner = AdventurerSpawner.Find();
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        // 冒険者が脱出や撃破された場合、冒険者側から呼び出して冒険の結果を報告する。
        public static void SetAdventureResult(int userID, bool isEscape, bool isSubGoalClear)
        {
            VantanConnect.UserRecord(userID, isEscape, isSubGoalClear);

            GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager._resultCount++;
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // 一度に生成する冒険者の最大数。
            const int Max = 4;

            while (!token.IsCancellationRequested)
            {
                // ゲーム開始。
                await VantanConnect.GameStart();
                token.ThrowIfCancellationRequested();

                _resultCount = 0;

                // 一定間隔で冒険者を生成。
                int spawnedCount = await _spawner.SpawnAsync(Max, token);

                // 生成した冒険者が全員、冒険の結果を報告するまで待つ。
                await UniTask.WaitUntil(() => _resultCount == spawnedCount, cancellationToken: token);

                // ゲーム終了。
                await VantanConnect.GameEnd();
                token.ThrowIfCancellationRequested();

                VantanConnect.SystemReset();
            }
        }
    }
}