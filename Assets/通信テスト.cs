using Cysharp.Threading.Tasks;
using Game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class 通信テスト : MonoBehaviour
{
    CancellationTokenSource _cts;

    void Start()
    {
        _cts = new CancellationTokenSource();
        AdventurerSpreadSheetLoader.GetDataAsync(_cts.Token).Forget();
        //_cts.Cancel();
    }
}
