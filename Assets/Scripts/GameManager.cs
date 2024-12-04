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

        // �`���҂��E�o�⌂�j���ꂽ�ꍇ�A�`���ґ�����Ăяo���Ė`���̌��ʂ�񍐂���B
        public static void ReportAdventureResult(Adventurer adventurer, string result)
        {
            GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager.SetAdventureResult(adventurer, result);
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // ��x�ɐ�������`���҂̍ő吔�B
            const int Max = 4;

            AdventurerSpawner.TryFind(out AdventurerSpawner spawner);
            while (!token.IsCancellationRequested)
            {
                _adventureResults.Clear();

                // ���Ԋu�Ŗ`���҂𐶐��B
                int spawnedCount = await spawner.SpawnAsync(Max, token);

                // ���������`���҂��S���A�`���̌��ʂ�񍐂���܂ő҂B
                await UniTask.WaitUntil(() => _adventureResults.Count == spawnedCount, cancellationToken: token);

                // �`���̌��ʂ𑗐M����B
                await AdventureResultSender.SendAsync(_adventureResults, token);
            }
        }

        void SetAdventureResult(Adventurer adventurer, string result)
        {
            _adventureResults.Add(adventurer, result);
        }
    }
}

// �`���̌��ʂ𑗐M����ۂɎ��S�����ꍇ�͖`���҂��[����悤�ȏ�����ǉ�����B
// ���̃^�C�~���O�ł̂ݕ�[����̂Ŕr�����䂪����Ȃ��Ȃ�H