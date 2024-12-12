using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class GASと通信 : MonoBehaviour
{
    void Start()
    {
        DoGetAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTask DoGetAsync(CancellationToken token)
    {
        const string URL = "https://script.google.com/macros/s/AKfycby6-OPwF24uUzoU9e9XFBRBZybEEZkMCKuayL8sjr39pooCzvYQaQzBQi2RIqpR7lu-QQ/exec";
        const string QueryParam = "?type=completion";
        using UnityWebRequest request = UnityWebRequest.Get(URL + QueryParam);
        await request.SendWebRequest().WithCancellation(token);

        string json = request.downloadHandler.text;
        Debug.Log(json);
    }
}
