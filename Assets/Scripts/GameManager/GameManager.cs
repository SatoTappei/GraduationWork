using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using VTNConnect;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public static event UnityAction OnGameStart;
        public static event UnityAction OnGameEnd;

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

        void OnDestroy()
        {
            OnGameStart = null;
            OnGameEnd = null;

            VantanConnect.GameEnd().Forget();
            VantanConnect.SystemReset();
        }

        // –`Œ¯Ò‚ª’Eo‚âŒ‚”j‚³‚ê‚½ê‡A–`Œ¯Ò‘¤‚©‚çŒÄ‚Ño‚µ‚Ä–`Œ¯‚ÌŒ‹‰Ê‚ğ•ñ‚·‚éB
        public static void SetAdventureResult(int userID, bool isEscape, bool isSubGoalClear)
        {
            VantanConnect.UserRecord(userID, isEscape, isSubGoalClear);

            GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager._resultCount++;
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // ƒQ[ƒ€ŠJnB
                GameStartAIGameResult gameStartResult = await VantanConnect.GameStart();
                token.ThrowIfCancellationRequested();
                OnGameStart?.Invoke();

                _resultCount = 0;

                // ˆê’èŠÔŠu‚Å–`Œ¯Ò‚ğ¶¬B
                int spawnedCount = await _spawner.SpawnAsync(gameStartResult.Artifacts, token);

                // ¶¬‚µ‚½–`Œ¯Ò‚ª‘SˆõA–`Œ¯‚ÌŒ‹‰Ê‚ğ•ñ‚·‚é‚Ü‚Å‘Ò‚ÂB
                await UniTask.WaitUntil(() => _resultCount == spawnedCount, cancellationToken: token);

                // ƒQ[ƒ€I—¹B
                await VantanConnect.GameEnd();
                token.ThrowIfCancellationRequested();
                OnGameEnd?.Invoke();

                VantanConnect.SystemReset();
            }
        }
    }
}