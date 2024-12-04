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

        // –`Œ¯Ò‚ª’Eo‚âŒ‚”j‚³‚ê‚½ê‡A–`Œ¯Ò‘¤‚©‚çŒÄ‚Ño‚µ‚Ä–`Œ¯‚ÌŒ‹‰Ê‚ğ•ñ‚·‚éB
        public static void ReportAdventureResult(Adventurer adventurer, string result)
        {
            GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager.SetAdventureResult(adventurer, result);
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // ˆê“x‚É¶¬‚·‚é–`Œ¯Ò‚ÌÅ‘å”B
            const int Max = 4;

            AdventurerSpawner.TryFind(out AdventurerSpawner spawner);
            while (!token.IsCancellationRequested)
            {
                _adventureResults.Clear();

                // ˆê’èŠÔŠu‚Å–`Œ¯Ò‚ğ¶¬B
                int spawnedCount = await spawner.SpawnAsync(Max, token);

                // ¶¬‚µ‚½–`Œ¯Ò‚ª‘SˆõA–`Œ¯‚ÌŒ‹‰Ê‚ğ•ñ‚·‚é‚Ü‚Å‘Ò‚ÂB
                await UniTask.WaitUntil(() => _adventureResults.Count == spawnedCount, cancellationToken: token);

                // –`Œ¯‚ÌŒ‹‰Ê‚ğ‘—M‚·‚éB
                await AdventureResultSender.SendAsync(_adventureResults, token);
            }
        }

        void SetAdventureResult(Adventurer adventurer, string result)
        {
            _adventureResults.Add(adventurer, result);
        }
    }
}

// –`Œ¯‚ÌŒ‹‰Ê‚ğ‘—M‚·‚éÛ‚É€–S‚µ‚½ê‡‚Í–`Œ¯Ò‚ğ•â[‚·‚é‚æ‚¤‚Èˆ—‚ğ’Ç‰Á‚·‚éB
// ‚»‚Ìƒ^ƒCƒ~ƒ“ƒO‚Å‚Ì‚İ•â[‚·‚é‚Ì‚Å”r‘¼§Œä‚ª‚¢‚ç‚È‚­‚È‚éH