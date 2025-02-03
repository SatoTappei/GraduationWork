using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Unused
{
    public static class AdventurerLoader
    {
        [System.Serializable]
        class Data
        {
            public int status;
            public AdventurerData[] result;
        }

        public static async UniTask<IReadOnlyList<AdventurerData>> GetDataAsync(CancellationToken token)
        {
            using UnityWebRequest request = UnityWebRequest.Get("https://vc.vtn-game.com/vc/gameusers");

            try
            {
                await request.SendWebRequest().WithCancellation(token);
            }
            catch(UnityWebRequestException e)
            {
                Debug.LogError(e);
                return new List<AdventurerData>();
            }

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(request.result);
                return new List<AdventurerData>();
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.result);
                return new List<AdventurerData>();
            }
            else
            {
                Data data = JsonUtility.FromJson<Data>(request.downloadHandler.text);
                return data.result.ToList();
            }
        }
    }
}
