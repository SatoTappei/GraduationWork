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

        // �`���҂��E�o�⌂�j���ꂽ�ꍇ�A�`���ґ�����Ăяo���Ė`���̌��ʂ�񍐂���B
        public static void SetAdventureResult(int userID, bool isEscape, bool isSubGoalClear)
        {
            VantanConnect.UserRecord(userID, isEscape, isSubGoalClear);

            GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager._resultCount++;
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // ��x�ɐ�������`���҂̍ő吔�B
            const int Max = 4;

            while (!token.IsCancellationRequested)
            {
                // �Q�[���J�n�B
                await VantanConnect.GameStart();
                token.ThrowIfCancellationRequested();

                _resultCount = 0;

                // ���Ԋu�Ŗ`���҂𐶐��B
                int spawnedCount = await _spawner.SpawnAsync(Max, token);

                // ���������`���҂��S���A�`���̌��ʂ�񍐂���܂ő҂B
                await UniTask.WaitUntil(() => _resultCount == spawnedCount, cancellationToken: token);

                // �Q�[���I���B
                await VantanConnect.GameEnd();
                token.ThrowIfCancellationRequested();

                VantanConnect.SystemReset();
            }
        }
    }
}