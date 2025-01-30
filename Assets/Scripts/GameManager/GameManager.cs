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
        Dictionary<Adventurer, string> _results;

        void Awake()
        {
            _spawner = AdventurerSpawner.Find();
            _results = new Dictionary<Adventurer, string>();
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        // –`Œ¯Ò‚ª’Eo‚âŒ‚”j‚³‚ê‚½ê‡A–`Œ¯Ò‘¤‚©‚çŒÄ‚Ño‚µ‚Ä–`Œ¯‚ÌŒ‹‰Ê‚ğ•ñ‚·‚éB
        public static void ReportAdventureResult(Adventurer adventurer, string result)
        {
            GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager._results.Add(adventurer, result);
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // ˆê“x‚É¶¬‚·‚é–`Œ¯Ò‚ÌÅ‘å”B
            const int Max = 4;

            while (!token.IsCancellationRequested)
            {
                // ƒQ[ƒ€ŠJnB
                await VantanConnect.GameStart();
                token.ThrowIfCancellationRequested();

                _results.Clear();

                // ˆê’èŠÔŠu‚Å–`Œ¯Ò‚ğ¶¬B
                int spawnedCount = await _spawner.SpawnAsync(Max, token);

                // ¶¬‚µ‚½–`Œ¯Ò‚ª‘SˆõA–`Œ¯‚ÌŒ‹‰Ê‚ğ•ñ‚·‚é‚Ü‚Å‘Ò‚ÂB
                await UniTask.WaitUntil(() => _results.Count == spawnedCount, cancellationToken: token);

                // ƒQ[ƒ€I—¹B
                await VantanConnect.GameEnd();
                token.ThrowIfCancellationRequested();
            }
        }
    }
}