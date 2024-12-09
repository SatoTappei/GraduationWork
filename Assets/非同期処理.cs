using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class 非同期処理 : MonoBehaviour
{
    void Start()
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        M(cts.Token).Forget();
        cts.Cancel();
    }

    async UniTask M(CancellationToken token)
    {
        await N(token);
        Debug.Log("M中1");
        await N(token);
        Debug.Log("M中2");

        //try
        //{
        //    await N(token);
        //    Debug.Log("M中");
        //}
        //catch
        //{
        //    Debug.Log("M中でキャンセル。");
        //}
    }

    async UniTask N(CancellationToken token)
    {
        await O(token);
        token.ThrowIfCancellationRequested();

        Debug.Log("N中");

        await Hoge(token);

        //try
        //{
        //    await O(token);
        //    Debug.Log("N中");
        //}
        //catch
        //{
        //    Debug.Log("N中でキャンセル。");
        //}
    }

    async UniTask O(CancellationToken token)
    {
        //await UniTask.WaitForSeconds(5.0f, cancellationToken: token);
        //Debug.Log("O中");

        try
        {
            await UniTask.WaitForSeconds(5.0f, cancellationToken: token);
            Debug.Log("O中");
        }
        catch
        {
            Debug.Log("O中でキャンセル。");
        }
    }

    async UniTask Hoge(CancellationToken token)
    {
        Debug.Log("Hogeはじめ");
        await UniTask.WaitForSeconds(1.0f, cancellationToken: token);
        Debug.Log("Hoge終わり");
    }
}
