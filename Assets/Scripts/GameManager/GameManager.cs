using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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

        // �`���҂��E�o�⌂�j���ꂽ�ꍇ�A�`���ґ�����Ăяo���Ė`���̌��ʂ�񍐂���B
        public static void ReportAdventureResult(Adventurer adventurer, string result)
        {
            GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager._adventureResults.Add(adventurer, result);
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // ��x�ɐ�������`���҂̍ő吔�B
            const int Max = 1;

            // "�g�p��"�̃X�v���b�h�V�[�g�̓��e�������B
            //await SpawnedAdventurerSender.DeleteAsync(token);

            while (!token.IsCancellationRequested)
            {
                _adventureResults.Clear();

                // ���Ԋu�Ŗ`���҂𐶐��B
                int spawnedCount = await _adventurerSpawner.SpawnAsync(Max, token);

                // ���������`���҂�"�g�p��"�̃X�v���b�h�V�[�g�ɏ������݁B
                //await SpawnedAdventurerSender.WriteAsync(spawner.Spawned, token);

                // ���������`���҂��S���A�`���̌��ʂ�񍐂���܂ő҂B
                await UniTask.WaitUntil(() => _adventureResults.Count == spawnedCount, cancellationToken: token);

                // �`���̌��ʂ𑗐M����B
                //await AdventureResultSender.SendAsync(_adventureResults, token);

                // "�g�p��"�̃X�v���b�h�V�[�g�̓��e�������B
                //await SpawnedAdventurerSender.DeleteAsync(token);
            }
        }
    }
}