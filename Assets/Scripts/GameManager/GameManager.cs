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

        // �`���҂��E�o�⌂�j���ꂽ�ꍇ�A�`���ґ�����Ăяo���Ė`���̌��ʂ�񍐂���B
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
                // �Q�[���J�n�B
                GameStartAIGameResult gameStartResult = await VantanConnect.GameStart();
                token.ThrowIfCancellationRequested();
                OnGameStart?.Invoke();

                _resultCount = 0;

                // ���Ԋu�Ŗ`���҂𐶐��B
                int spawnedCount = await _spawner.SpawnAsync(gameStartResult.Artifacts, token);

                // ���������`���҂��S���A�`���̌��ʂ�񍐂���܂ő҂B
                await UniTask.WaitUntil(() => _resultCount == spawnedCount, cancellationToken: token);

                // �Q�[���I���B
                await VantanConnect.GameEnd();
                token.ThrowIfCancellationRequested();
                OnGameEnd?.Invoke();

                VantanConnect.SystemReset();
            }
        }
    }
}