using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using VTNConnect;

namespace Game
{
    public class AdventurerView : MonoBehaviour, IVantanConnectEventReceiver
    {
        [SerializeField] AdventurerViewUI[] _ui;

        bool _isActive;

        public bool IsActive => _isActive;

        void Awake()
        {
            VantanConnect.RegisterEventReceiver(this);
        }

        void Start()
        {
            _isActive = true;
        }

        public async void OnEventCall(EventData data)
        {
            Debug.Log($"WebSocket Event Received: {data.EventCode}");

            if (data.EventCode == EventDefine.GameStart && data.GetIntData("GameId") == 1)
            {
                APIGetGameUsersImplement users = new APIGetGameUsersImplement();
                GetActiveGameUsersResult userData = await users.Request();

                // 取得した冒険者のデータが5人以上いる可能性を考慮。
                int length = Mathf.Min(userData.UserData.Length, 4);
                for (int i = 0; i < length; i++)
                {
                    _ui[i].SetProfile(
                        userData.UserData[i].Name,
                        userData.UserData[i].Gender,
                        userData.UserData[i].Age.ToString(),
                        userData.UserData[i].Job,
                        userData.UserData[i].Personality,
                        userData.UserData[i].Motivation,
                        userData.UserData[i].Weaknesses,
                        userData.UserData[i].Background
                    );
                }
            }
            else if (data.EventCode == EventDefine.GameEnd)
            {
                //
            }
        }
    }
}
