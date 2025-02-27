using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class LineDisplayer : MonoBehaviour
    {
        RolePlay _rolePlay;
        StatusBarBinder _statusBar;

        void Awake()
        {
            _rolePlay = GetComponent<RolePlay>();
            _statusBar = GetComponent<StatusBarBinder>();
        }

        public void Show(RequestLineType type)
        {
            ShowAsync(type, this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask ShowAsync(RequestLineType type, CancellationToken token)
        {
            string line = await _rolePlay.RequestLineAsync(type, token);
            token.ThrowIfCancellationRequested();

            _statusBar.ShowLine(line);
        }
    }
}
