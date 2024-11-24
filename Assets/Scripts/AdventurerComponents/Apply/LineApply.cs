using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class LineApply : MonoBehaviour
    {
        public void ShowLine(RequestLineType type)
        {
            ShowLineAsync(type, this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask ShowLineAsync(RequestLineType type, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!TryGetComponent(out RolePlayAI ai)) return;
            if (!TryGetComponent(out StatusBarApply statusBar)) return; 

            string line = await ai.RequestLineAsync(type, token);
            statusBar.ShowLine(line);
        }
    }
}
